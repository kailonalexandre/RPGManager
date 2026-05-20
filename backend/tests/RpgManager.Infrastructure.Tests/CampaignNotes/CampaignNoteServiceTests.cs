using Microsoft.EntityFrameworkCore;
using RpgManager.Application.CampaignNotes;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.CampaignNotes;
using RpgManager.Infrastructure.Data;
using RpgManager.Infrastructure.Permissions;
using Xunit;

namespace RpgManager.Infrastructure.Tests.CampaignNotes;

public sealed class CampaignNoteServiceTests
{
    [Fact]
    public async Task Master_creates_note_as_master_only_by_default()
    {
        await using var dbContext = CreateDbContext();
        var (master, _, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(
            master.Id,
            campaign.Id,
            Request("Plano da sessão", "Conteúdo"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(Visibility.MasterOnly, result.Data!.Visibility);
        Assert.Equal(master.Id, result.Data.OwnerUserId);
    }

    [Fact]
    public async Task Player_creates_note_as_private_by_default()
    {
        await using var dbContext = CreateDbContext();
        var (_, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(
            player.Id,
            campaign.Id,
            Request("Diário", "Anotação do jogador"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(Visibility.Private, result.Data!.Visibility);
        Assert.Equal(player.Id, result.Data.OwnerUserId);
    }

    [Fact]
    public async Task Player_cannot_view_master_only_note()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        var created = await service.CreateAsync(
            master.Id,
            campaign.Id,
            Request("Segredo", "Só mestre"),
            CancellationToken.None);

        var getResult = await service.GetByIdAsync(player.Id, campaign.Id, created.Data!.Id, CancellationToken.None);
        var listResult = await service.GetAsync(
            player.Id,
            campaign.Id,
            new CampaignNoteQuery(null, null, null, null, null),
            CancellationToken.None);

        Assert.False(getResult.Succeeded);
        Assert.Empty(listResult.Data!);
    }

    [Fact]
    public async Task Outsider_cannot_view_public_to_players_note()
    {
        await using var dbContext = CreateDbContext();
        var (master, _, outsider, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        var created = await service.CreateAsync(
            master.Id,
            campaign.Id,
            Request("Publicado", "Visível aos jogadores", visibility: Visibility.PublicToPlayers),
            CancellationToken.None);

        var result = await service.GetByIdAsync(outsider.Id, campaign.Id, created.Data!.Id, CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Master_can_view_player_private_note()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        await service.CreateAsync(
            player.Id,
            campaign.Id,
            Request("Diário", "Nota privada"),
            CancellationToken.None);

        var result = await service.GetAsync(
            master.Id,
            campaign.Id,
            new CampaignNoteQuery(null, null, null, null, null),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Single(result.Data!);
        Assert.Equal(player.Id, result.Data![0].OwnerUserId);
    }

    [Fact]
    public async Task Search_and_tag_filters_work()
    {
        await using var dbContext = CreateDbContext();
        var (master, _, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);
        await service.CreateAsync(
            master.Id,
            campaign.Id,
            Request("Taverna antiga", "Rumores da vila", "npc,lugar"),
            CancellationToken.None);
        await service.CreateAsync(
            master.Id,
            campaign.Id,
            Request("Tesouro", "Mapa secreto", "item"),
            CancellationToken.None);

        var searchResult = await service.GetAsync(
            master.Id,
            campaign.Id,
            new CampaignNoteQuery("taverna", null, null, null, null),
            CancellationToken.None);
        var tagResult = await service.GetAsync(
            master.Id,
            campaign.Id,
            new CampaignNoteQuery(null, "item", null, null, null),
            CancellationToken.None);

        Assert.Single(searchResult.Data!);
        Assert.Equal("Taverna antiga", searchResult.Data![0].Title);
        Assert.Single(tagResult.Data!);
        Assert.Equal("Tesouro", tagResult.Data![0].Title);
    }

    [Fact]
    public async Task Player_cannot_create_public_note()
    {
        await using var dbContext = CreateDbContext();
        var (_, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(
            player.Id,
            campaign.Id,
            Request("Publicar", "Tentativa", visibility: Visibility.PublicToPlayers),
            CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    private static CampaignNoteService CreateService(AppDbContext dbContext)
        => new(dbContext, new CampaignPermissionService(dbContext));

    private static CampaignNoteRequest Request(
        string title,
        string content,
        string tags = "",
        Visibility? visibility = null)
        => new(
            title,
            content,
            tags,
            visibility,
            null,
            null,
            ExternalProvider.None,
            null);

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
