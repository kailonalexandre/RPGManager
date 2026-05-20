using RpgManager.Application.Common;

namespace RpgManager.Application.CampaignNotes;

public interface ICampaignNoteService
{
    Task<ServiceResult<IReadOnlyList<CampaignNoteResponse>>> GetAsync(
        Guid userId,
        Guid campaignId,
        CampaignNoteQuery query,
        CancellationToken cancellationToken);

    Task<ServiceResult<CampaignNoteResponse>> GetByIdAsync(
        Guid userId,
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken);

    Task<ServiceResult<CampaignNoteResponse>> CreateAsync(
        Guid userId,
        Guid campaignId,
        CampaignNoteRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResult<CampaignNoteResponse>> UpdateAsync(
        Guid userId,
        Guid campaignId,
        Guid noteId,
        CampaignNoteRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken);
}
