using RpgManager.Domain.Enums;

namespace RpgManager.Application.Permissions;

public interface ICampaignPermissionService
{
    Task<bool> IsCampaignMemberAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsCampaignMasterAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<CampaignRole?> GetCampaignRoleAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanViewCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<Visibility?> GetDefaultVisibilityAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanViewCampaignScopedContentAsync(
        Guid campaignId,
        Guid ownerUserId,
        Visibility visibility,
        Guid userId,
        CancellationToken cancellationToken);
    Task<bool> CanEditCampaignScopedContentAsync(
        Guid campaignId,
        Guid ownerUserId,
        Visibility visibility,
        Guid userId,
        CancellationToken cancellationToken);
}
