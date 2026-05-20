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

    public Task<bool> CanViewCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken)
        => IsCampaignMemberAsync(campaignId, userId, cancellationToken);

    public Task<bool> CanEditCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken)
        => IsCampaignMasterAsync(campaignId, userId, cancellationToken);
}
