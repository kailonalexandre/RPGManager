using RpgManager.Application.Common;

namespace RpgManager.Application.Campaigns;

public interface ICampaignService
{
    Task<IReadOnlyList<CampaignSummaryResponse>> GetMyCampaignsAsync(Guid userId, CancellationToken cancellationToken);
    Task<ServiceResult<CampaignResponse>> GetByIdAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken);
    Task<ServiceResult<CampaignResponse>> CreateAsync(Guid userId, CampaignRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CampaignResponse>> UpdateAsync(Guid userId, Guid campaignId, CampaignRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken);
    Task<ServiceResult<CampaignResponse>> JoinAsync(Guid userId, JoinCampaignRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CampaignResponse>> RegenerateInviteAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CampaignMemberResponse>>> GetMembersAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CampaignCharacterSummaryResponse>>> GetCharactersAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken);
    Task<ServiceResult<CampaignMasterDashboardResponse>> GetMasterDashboardAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken);
}
