using RpgManager.Application.Common;

namespace RpgManager.Application.Npcs;

public interface INpcService
{
    Task<ServiceResult<IReadOnlyList<NpcResponse>>> GetAsync(
        Guid userId,
        Guid campaignId,
        NpcQuery query,
        CancellationToken cancellationToken);

    Task<ServiceResult<NpcResponse>> GetByIdAsync(
        Guid userId,
        Guid campaignId,
        Guid npcId,
        CancellationToken cancellationToken);

    Task<ServiceResult<NpcResponse>> CreateAsync(
        Guid userId,
        Guid campaignId,
        NpcRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResult<NpcResponse>> UpdateAsync(
        Guid userId,
        Guid campaignId,
        Guid npcId,
        NpcRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid campaignId,
        Guid npcId,
        CancellationToken cancellationToken);
}
