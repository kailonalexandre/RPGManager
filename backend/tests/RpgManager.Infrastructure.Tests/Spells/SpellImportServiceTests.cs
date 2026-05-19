using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;
using RpgManager.Infrastructure.Spells;
using Xunit;

namespace RpgManager.Infrastructure.Tests.Spells;

public sealed class SpellImportServiceTests
{
    [Fact]
    public async Task ImportOpen5eAsync_creates_new_imported_spell()
    {
        await using var dbContext = CreateDbContext();
        var userId = await SeedUserAsync(dbContext);
        var service = CreateService(dbContext, SpellPage(SpellItem("srd-2024_acid-arrow")));

        var result = await service.ImportOpen5eAsync(userId, CancellationToken.None);

        var spell = await dbContext.Spells.SingleAsync();
        Assert.Equal(1, result.Created);
        Assert.Equal(0, result.Updated);
        Assert.False(spell.IsHomebrew);
        Assert.True(spell.IsImported);
        Assert.True(spell.IsSrd);
        Assert.Equal("Open5e", spell.ExternalSource);
        Assert.Equal("srd-2024_acid-arrow", spell.ExternalId);
        Assert.Equal("D&D 2024", spell.RulesVersion);
        Assert.Equal("en", spell.Language);
        Assert.True(spell.TranslationMissing);
        Assert.Equal(SpellVisibility.LocalPublic, spell.Visibility);
    }

    [Fact]
    public async Task ImportOpen5eAsync_updates_existing_imported_spell_without_duplicate()
    {
        await using var dbContext = CreateDbContext();
        var userId = await SeedUserAsync(dbContext);
        var first = CreateService(dbContext, SpellPage(SpellItem("srd-2024_acid-arrow", description: "Old")));
        await first.ImportOpen5eAsync(userId, CancellationToken.None);

        var second = CreateService(dbContext, SpellPage(SpellItem("srd-2024_acid-arrow", description: "New")));
        var result = await second.ImportOpen5eAsync(userId, CancellationToken.None);

        var spell = await dbContext.Spells.SingleAsync();
        Assert.Equal(0, result.Created);
        Assert.Equal(1, result.Updated);
        Assert.Equal("New", spell.Description);
    }

    [Fact]
    public async Task ImportOpen5eAsync_skips_homebrew_conflict()
    {
        await using var dbContext = CreateDbContext();
        var userId = await SeedUserAsync(dbContext);
        dbContext.Spells.Add(new Spell
        {
            Name = "Acid Arrow",
            EnglishName = "Acid Arrow",
            Level = 2,
            School = "Evocação",
            Source = "Homebrew",
            IsHomebrew = true,
            CreatedByUserId = userId,
            Visibility = SpellVisibility.Private
        });
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext, SpellPage(SpellItem("srd-2024_acid-arrow")));
        var result = await service.ImportOpen5eAsync(userId, CancellationToken.None);

        Assert.Equal(0, result.Created);
        Assert.Equal(1, result.Skipped);
        Assert.Equal(1, await dbContext.Spells.CountAsync());
    }

    [Fact]
    public void Open5eSpellMapper_maps_core_fields()
    {
        var item = SpellItem(
            "srd-2024_acid-arrow",
            school: "Evocation",
            classes: ["Wizard", "Sorcerer"],
            concentration: true,
            ritual: true,
            verbal: true,
            somatic: true,
            material: true);
        var spell = new Spell();

        Open5eSpellMapper.Apply(spell, item, Guid.NewGuid(), DateTime.UtcNow);

        Assert.Equal(2, spell.Level);
        Assert.Equal("Evocação", spell.School);
        Assert.Equal("V, S, M", spell.Components);
        Assert.True(spell.IsConcentration);
        Assert.True(spell.IsRitual);
        Assert.Contains("Mago", spell.AvailableClasses);
        Assert.Contains("Feiticeiro", spell.AvailableClasses);
    }

    [Fact]
    public async Task ImportOpen5eAsync_handles_api_error_without_throwing()
    {
        await using var dbContext = CreateDbContext();
        var userId = await SeedUserAsync(dbContext);
        var service = CreateService(dbContext, null);

        var result = await service.ImportOpen5eAsync(userId, CancellationToken.None);

        Assert.Equal(0, result.Created);
        Assert.NotEmpty(result.Errors);
    }

    private static SpellImportService CreateService(AppDbContext dbContext, Open5eSpellPage? page)
    {
        return new SpellImportService(
            dbContext,
            new FakeOpen5eSpellClient(page),
            Options.Create(new Open5eSpellImportOptions { MaxPages = 2, PageSize = 10 }),
            NullLogger<SpellImportService>.Instance);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<Guid> SeedUserAsync(AppDbContext dbContext)
    {
        var user = new User
        {
            Name = "GM",
            Email = $"{Guid.NewGuid():N}@example.com",
            PasswordHash = "hash",
            Profile = UserProfile.GameMaster
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user.Id;
    }

    private static Open5eSpellPage SpellPage(Open5eSpellItem item)
        => new(1, null, null, [item]);

    private static Open5eSpellItem SpellItem(
        string key,
        string description = "Description",
        string school = "Evocation",
        IReadOnlyList<string>? classes = null,
        bool concentration = false,
        bool ritual = false,
        bool verbal = true,
        bool somatic = true,
        bool material = false)
    {
        return new Open5eSpellItem(
            key,
            new Open5eDocument(
                "System Reference Document 5.2",
                "srd-2024",
                "5e 2024 Rules",
                new Open5eGameSystem("5th Edition 2024", "5e-2024")),
            new Open5eSchool(school, school.ToLowerInvariant()),
            (classes ?? ["Wizard"]).Select(item => new Open5eClass(item, item.ToLowerInvariant())).ToList(),
            "Acid Arrow",
            description,
            2,
            "Higher",
            "90 feet",
            ritual,
            "action",
            verbal,
            somatic,
            material,
            material ? "powdered leaf" : "",
            "Instantaneous",
            concentration);
    }

    private sealed class FakeOpen5eSpellClient(Open5eSpellPage? page) : IOpen5eSpellClient
    {
        public Task<Open5eSpellPage?> GetPageAsync(string url, CancellationToken cancellationToken)
            => Task.FromResult(page);
    }
}
