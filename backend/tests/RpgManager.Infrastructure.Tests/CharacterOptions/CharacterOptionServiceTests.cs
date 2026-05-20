using Microsoft.EntityFrameworkCore;
using RpgManager.Application.CharacterOptions;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.CharacterOptions;
using RpgManager.Infrastructure.Data;
using Xunit;

namespace RpgManager.Infrastructure.Tests.CharacterOptions;

public sealed class CharacterOptionServiceTests
{
    [Fact]
    public async Task Creates_and_lists_races_classes_and_backgrounds()
    {
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var service = new CharacterOptionService(dbContext);

        var race = await service.CreateRaceAsync(
            user.Id,
            new RaceRequest("Povo da Névoa", "Opção fictícia.", "Homebrew local", true),
            CancellationToken.None);
        var characterClass = await service.CreateClassAsync(
            user.Id,
            new CharacterClassRequest("Guardião das Pontes", 10, "Classe fictícia.", "Homebrew local", true),
            CancellationToken.None);
        var background = await service.CreateBackgroundAsync(
            user.Id,
            new BackgroundRequest("Cartógrafo Errante", "Antecedente fictício.", "Homebrew local", true),
            CancellationToken.None);

        Assert.True(race.Succeeded);
        Assert.True(characterClass.Succeeded);
        Assert.True(background.Succeeded);
        Assert.Single(await service.GetRacesAsync(CancellationToken.None));
        Assert.Single(await service.GetClassesAsync(CancellationToken.None));
        Assert.Single(await service.GetBackgroundsAsync(CancellationToken.None));
    }

    [Fact]
    public async Task Rejects_invalid_character_class_hit_die()
    {
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext);
        var service = new CharacterOptionService(dbContext);

        var result = await service.CreateClassAsync(
            user.Id,
            new CharacterClassRequest("Classe", 0, "", "", true),
            CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Player_cannot_create_character_options()
    {
        await using var dbContext = CreateDbContext();
        var user = await SeedUserAsync(dbContext, UserProfile.Player);
        var service = new CharacterOptionService(dbContext);

        var result = await service.CreateRaceAsync(
            user.Id,
            new RaceRequest("Opção", "", "", true),
            CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<User> SeedUserAsync(
        AppDbContext dbContext,
        UserProfile profile = UserProfile.GameMaster)
    {
        var user = new User
        {
            Name = "master",
            Email = "master@example.com",
            PasswordHash = "hash",
            Profile = profile
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}
