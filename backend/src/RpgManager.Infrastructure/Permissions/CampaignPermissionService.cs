using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Permissions;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Permissions;

public sealed class CampaignPermissionService(AppDbContext dbContext) : ICampaignPermissionService
{
    public Task<bool> IsCampaignMemberAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken)
        => dbContext.CampaignMembers.AnyAsync(
            member => member.CampaignId == campaignId && member.UserId == userId,
            cancellationToken);

    public Task<bool> IsCampaignMasterAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken)
        => dbContext.CampaignMembers.AnyAsync(
            member => member.CampaignId == campaignId &&
                member.UserId == userId &&
                member.Role == CampaignRole.Master,
            cancellationToken);

    public async Task<CampaignRole?> GetCampaignRoleAsync(
        Guid campaignId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var role = await dbContext.CampaignMembers
            .AsNoTracking()
            .Where(member => member.CampaignId == campaignId && member.UserId == userId)
            .Select(member => (CampaignRole?)member.Role)
            .SingleOrDefaultAsync(cancellationToken);

        return role;
    }

    public Task<bool> CanViewCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken)
        => IsCampaignMemberAsync(campaignId, userId, cancellationToken);

    public Task<bool> CanEditCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken)
        => IsCampaignMasterAsync(campaignId, userId, cancellationToken);

    public async Task<Visibility?> GetDefaultVisibilityAsync(
        Guid campaignId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var role = await GetCampaignRoleAsync(campaignId, userId, cancellationToken);

        return role switch
        {
            CampaignRole.Master => Visibility.MasterOnly,
            CampaignRole.Player => Visibility.Private,
            _ => null
        };
    }

    public async Task<bool> CanViewCampaignScopedContentAsync(
        Guid campaignId,
        Guid ownerUserId,
        Visibility visibility,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var role = await GetCampaignRoleAsync(campaignId, userId, cancellationToken);
        if (role is null)
        {
            return false;
        }

        if (role == CampaignRole.Master)
        {
            return true;
        }

        return visibility switch
        {
            Visibility.Private => ownerUserId == userId,
            Visibility.Campaign => true,
            Visibility.MasterOnly => false,
            Visibility.PlayerOnly => ownerUserId == userId,
            Visibility.PublicToPlayers => true,
            _ => false
        };
    }

    public async Task<bool> CanEditCampaignScopedContentAsync(
        Guid campaignId,
        Guid ownerUserId,
        Visibility visibility,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var role = await GetCampaignRoleAsync(campaignId, userId, cancellationToken);
        if (role is null)
        {
            return false;
        }

        if (role == CampaignRole.Master)
        {
            return true;
        }

        return ownerUserId == userId &&
            (visibility == Visibility.Private || visibility == Visibility.PlayerOnly);
    }
}
