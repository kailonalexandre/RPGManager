using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Npcs;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;
using RpgManager.Infrastructure.Npcs;
using RpgManager.Infrastructure.Permissions;
using Xunit;

namespace RpgManager.Infrastructure.Tests.Npcs;

public sealed class NpcServiceTests
{
    [Fact]
    public async Task Master_creates_npc()
    {
        await using var dbContext = CreateDbContext();
        var (master, _, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(master.Id, campaign.Id, Request("Mira"), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal("Mira", result.Data!.Name);
        Assert.Equal(master.Id, result.Data.CreatedByUserId);
    }

    [Fact]
    public async Task Player_cannot_create_npc()
    {
        await using var dbContext = CreateDbContext();
        var (_, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(player.Id, campaign.Id, Request("Mira"), CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Player_cannot_view_master_only_npc()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        var created = await service.CreateAsync(master.Id, campaign.Id, Request("Segredo"), CancellationToken.None);

        var getResult = await service.GetByIdAsync(player.Id, campaign.Id, created.Data!.Id, CancellationToken.None);
        var listResult = await service.GetAsync(player.Id, campaign.Id, EmptyQuery(), CancellationToken.None);

        Assert.False(getResult.Succeeded);
        Assert.Empty(listResult.Data!);
    }

    [Fact]
    public async Task Player_sees_public_npc_without_secrets()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        await service.CreateAsync(
            master.Id,
            campaign.Id,
            Request("Publicado", visibility: Visibility.PublicToPlayers, secrets: "Segredo real"),
            CancellationToken.None);

        var result = await service.GetAsync(player.Id, campaign.Id, EmptyQuery(), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Single(result.Data!);
        Assert.Null(result.Data![0].Secrets);
    }

    [Fact]
    public async Task Master_sees_all_npcs_and_secrets()
    {
        await using var dbContext = CreateDbContext();
        var (master, _, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        await service.CreateAsync(master.Id, campaign.Id, Request("Privado", secrets: "Segredo"), CancellationToken.None);

        var result = await service.GetAsync(master.Id, campaign.Id, EmptyQuery(), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Single(result.Data!);
        Assert.Equal("Segredo", result.Data![0].Secrets);
    }

    [Fact]
    public async Task Search_and_faction_filters_work()
    {
        await using var dbContext = CreateDbContext();
        var (master, _, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        await service.CreateAsync(master.Id, campaign.Id, Request("Mira", faction: "Guilda Azul"), CancellationToken.None);
        await service.CreateAsync(master.Id, campaign.Id, Request("Toran", faction: "Mercado Livre"), CancellationToken.None);

        var searchResult = await service.GetAsync(
            master.Id,
            campaign.Id,
            new NpcQuery("mira", null, null, null, null, null, null),
            CancellationToken.None);
        var factionResult = await service.GetAsync(
            master.Id,
            campaign.Id,
            new NpcQuery(null, null, null, "mercado", null, null, null),
            CancellationToken.None);

        Assert.Single(searchResult.Data!);
        Assert.Equal("Mira", searchResult.Data![0].Name);
        Assert.Single(factionResult.Data!);
        Assert.Equal("Toran", factionResult.Data![0].Name);
    }

    private static NpcService CreateService(AppDbContext dbContext)
        => new(dbContext, new CampaignPermissionService(dbContext));

    private static NpcQuery EmptyQuery()
        => new(null, null, null, null, null, null, null);

    private static NpcRequest Request(
        string name,
        Visibility visibility = Visibility.MasterOnly,
        string secrets = "",
        string faction = "")
        => new(
            name,
            "",
            "",
            "",
            "Porto",
            faction,
            "",
            "",
            "",
            secrets,
            "",
            "",
            "",
            false,
            true,
            visibility);

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<(User Master, User Player, User Outsider, Campaign Campaign)> SeedCampaignAsync(
        AppDbContext dbContext)
    {
        var master = SeedUser("master@example.com", UserProfile.GameMaster);
        var player = SeedUser("player@example.com", UserProfile.Player);
        var outsider = SeedUser("outsider@example.com", UserProfile.Player);
        var campaign = new Campaign
        {
            Name = "Campaign",
            Description = "Description",
            System = "D&D 5e",
            InviteCode = Guid.NewGuid().ToString("N")[..8],
            CreatedByUser = master,
            Members =
            [
                new CampaignMember { User = master, Role = CampaignRole.Master },
                new CampaignMember { User = player, Role = CampaignRole.Player }
            ]
        };

        dbContext.Users.AddRange(master, player, outsider);
        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync();
        return (master, player, outsider, campaign);
    }

    private static User SeedUser(string email, UserProfile profile)
        => new()
        {
            Name = email.Split('@')[0],
            Email = email,
            PasswordHash = "hash",
            Profile = profile
        };
}
