using RpgManager.Application.Common;
using RpgManager.Application.Spells;

namespace RpgManager.Application.Features;

public interface IFeatureService
{
    Task<PagedResponse<FeatureResponse>> GetVisibleAsync(Guid userId, FeatureFilters filters, CancellationToken cancellationToken);
    Task<ServiceResult<FeatureResponse>> GetByIdAsync(Guid userId, Guid featureId, CancellationToken cancellationToken);
    Task<ServiceResult<FeatureResponse>> CreateAsync(Guid userId, FeatureRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<FeatureResponse>> UpdateAsync(Guid userId, Guid featureId, FeatureRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteAsync(Guid userId, Guid featureId, CancellationToken cancellationToken);
}
