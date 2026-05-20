namespace RpgManager.Application.Permissions;

public interface ICampaignPermissionService
{
    Task<bool> IsCampaignMemberAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsCampaignMasterAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanViewCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditCampaignAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken);
}
