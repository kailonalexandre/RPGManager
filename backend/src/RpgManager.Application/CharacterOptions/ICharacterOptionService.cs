using RpgManager.Application.Common;

namespace RpgManager.Application.CharacterOptions;

public interface ICharacterOptionService
{
    Task<IReadOnlyList<RaceResponse>> GetRacesAsync(CancellationToken cancellationToken);
    Task<ServiceResult<RaceResponse>> CreateRaceAsync(Guid userId, RaceRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<CharacterClassResponse>> GetClassesAsync(CancellationToken cancellationToken);
    Task<ServiceResult<CharacterClassResponse>> CreateClassAsync(Guid userId, CharacterClassRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<BackgroundResponse>> GetBackgroundsAsync(CancellationToken cancellationToken);
    Task<ServiceResult<BackgroundResponse>> CreateBackgroundAsync(Guid userId, BackgroundRequest request, CancellationToken cancellationToken);
}
