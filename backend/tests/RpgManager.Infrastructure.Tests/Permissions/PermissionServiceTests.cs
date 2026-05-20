using Microsoft.EntityFrameworkCore;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;
using RpgManager.Infrastructure.Permissions;
using Xunit;

namespace RpgManager.Infrastructure.Tests.Permissions;

public sealed class PermissionServiceTests
{
    [Fact]
    public async Task Player_cannot_view_other_player_character_in_same_campaign()
    {
        await using var dbContext = CreateDbContext();
        var (_, playerA, playerB, campaign) = await SeedCampaignAsync(dbContext);
        var character = await SeedCharacterAsync(dbContext, playerA.Id, campaign.Id);
        var service = new CharacterPermissionService(dbContext);

        var canView = await service.CanViewCharacterAsync(character.Id, playerB.Id, CancellationToken.None);

        Assert.False(canView);
    }

    [Fact]
    public async Task Master_can_view_only_non_private_visible_note()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, _, campaign) = await SeedCampaignAsync(dbContext);
        var character = await SeedCharacterAsync(dbContext, player.Id, campaign.Id);
        var visibleNote = new CharacterNote
        {
            CharacterId = character.Id,
            Title = "Visible",
            IsPrivate = false,
            IsVisibleToMaster = true
        };
        var privateNote = new CharacterNote
        {
            CharacterId = character.Id,
            Title = "Private",
            IsPrivate = true,
            IsVisibleToMaster = false
        };
        dbContext.CharacterNotes.AddRange(visibleNote, privateNote);
        await dbContext.SaveChangesAsync();
        var service = new CharacterPermissionService(dbContext);

        var canViewVisible = await service.CanViewNoteAsync(visibleNote.Id, master.Id, CancellationToken.None);
        var canViewPrivate = await service.CanViewNoteAsync(privateNote.Id, master.Id, CancellationToken.None);

        Assert.True(canViewVisible);
        Assert.False(canViewPrivate);
    }

    [Fact]
    public async Task Content_visibility_respects_private_campaign_and_local_public_rules()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, outsider, campaign) = await SeedCampaignAsync(dbContext);
        var privateSpell = SeedSpell(master.Id, SpellVisibility.Private);
        var campaignSpell = SeedSpell(master.Id, SpellVisibility.Campaign, campaign.Id);
        var publicSpell = SeedSpell(master.Id, SpellVisibility.LocalPublic);
        dbContext.Spells.AddRange(privateSpell, campaignSpell, publicSpell);
        await dbContext.SaveChangesAsync();
        var service = new ContentVisibilityService(dbContext);

        Assert.True(await service.CanViewSpellAsync(privateSpell.Id, master.Id, CancellationToken.None));
        Assert.False(await service.CanViewSpellAsync(privateSpell.Id, player.Id, CancellationToken.None));
        Assert.True(await service.CanViewSpellAsync(campaignSpell.Id, player.Id, CancellationToken.None));
        Assert.False(await service.CanViewSpellAsync(campaignSpell.Id, outsider.Id, CancellationToken.None));
        Assert.True(await service.CanViewSpellAsync(publicSpell.Id, outsider.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Campaign_permission_returns_role_and_default_visibility()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, outsider, campaign) = await SeedCampaignAsync(dbContext);
        var service = new CampaignPermissionService(dbContext);

        Assert.Equal(CampaignRole.Master, await service.GetCampaignRoleAsync(campaign.Id, master.Id, CancellationToken.None));
        Assert.Equal(CampaignRole.Player, await service.GetCampaignRoleAsync(campaign.Id, player.Id, CancellationToken.None));
        Assert.Null(await service.GetCampaignRoleAsync(campaign.Id, outsider.Id, CancellationToken.None));
        Assert.Equal(Visibility.MasterOnly, await service.GetDefaultVisibilityAsync(campaign.Id, master.Id, CancellationToken.None));
        Assert.Equal(Visibility.Private, await service.GetDefaultVisibilityAsync(campaign.Id, player.Id, CancellationToken.None));
        Assert.Null(await service.GetDefaultVisibilityAsync(campaign.Id, outsider.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Campaign_scoped_visibility_keeps_master_and_player_access_separated()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, outsider, campaign) = await SeedCampaignAsync(dbContext);
        var service = new CampaignPermissionService(dbContext);

        Assert.True(await service.CanViewCampaignScopedContentAsync(
            campaign.Id,
            player.Id,
            Visibility.Private,
            master.Id,
            CancellationToken.None));
        Assert.True(await service.CanViewCampaignScopedContentAsync(
            campaign.Id,
            player.Id,
            Visibility.Private,
            player.Id,
            CancellationToken.None));
        Assert.False(await service.CanViewCampaignScopedContentAsync(
            campaign.Id,
            master.Id,
            Visibility.MasterOnly,
            player.Id,
            CancellationToken.None));
        Assert.True(await service.CanViewCampaignScopedContentAsync(
            campaign.Id,
            master.Id,
            Visibility.PublicToPlayers,
            player.Id,
            CancellationToken.None));
        Assert.False(await service.CanViewCampaignScopedContentAsync(
            campaign.Id,
            master.Id,
            Visibility.PublicToPlayers,
            outsider.Id,
            CancellationToken.None));
    }

    [Fact]
    public async Task Campaign_scoped_editing_allows_master_or_owner_private_content()
    {
        await using var dbContext = CreateDbContext();
        var (master, player, outsider, campaign) = await SeedCampaignAsync(dbContext);
        var service = new CampaignPermissionService(dbContext);

        Assert.True(await service.CanEditCampaignScopedContentAsync(
            campaign.Id,
            player.Id,
            Visibility.Private,
            master.Id,
            CancellationToken.None));
        Assert.True(await service.CanEditCampaignScopedContentAsync(
            campaign.Id,
            player.Id,
            Visibility.Private,
            player.Id,
            CancellationToken.None));
        Assert.False(await service.CanEditCampaignScopedContentAsync(
            campaign.Id,
            master.Id,
            Visibility.MasterOnly,
            player.Id,
            CancellationToken.None));
        Assert.False(await service.CanEditCampaignScopedContentAsync(
            campaign.Id,
            player.Id,
            Visibility.Private,
            outsider.Id,
            CancellationToken.None));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<(User Master, User PlayerA, User PlayerB, Campaign Campaign)> SeedCampaignAsync(AppDbContext dbContext)
    {
        var master = SeedUser("master@example.com", UserProfile.GameMaster);
        var playerA = SeedUser("player-a@example.com", UserProfile.Player);
        var playerB = SeedUser("player-b@example.com", UserProfile.Player);
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
                new CampaignMember { User = playerA, Role = CampaignRole.Player }
            ]
        };

        dbContext.Users.AddRange(master, playerA, playerB);
        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync();
        return (master, playerA, playerB, campaign);
    }

    private static User SeedUser(string email, UserProfile profile)
        => new()
        {
            Name = email.Split('@')[0],
            Email = email,
            PasswordHash = "hash",
            Profile = profile
        };

    private static async Task<Character> SeedCharacterAsync(AppDbContext dbContext, Guid userId, Guid campaignId)
    {
        var character = new Character
        {
            UserId = userId,
            CampaignId = campaignId,
            Name = "Character",
            Species = "Human",
            MainClass = "Fighter"
        };

        dbContext.Characters.Add(character);
        await dbContext.SaveChangesAsync();
        return character;
    }

    private static Spell SeedSpell(Guid userId, SpellVisibility visibility, Guid? campaignId = null)
        => new()
        {
            Name = Guid.NewGuid().ToString("N"),
            School = "Evocation",
            CreatedByUserId = userId,
            Visibility = visibility,
            CampaignId = campaignId
        };
}
