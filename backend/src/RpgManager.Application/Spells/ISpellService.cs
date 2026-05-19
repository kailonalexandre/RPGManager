using RpgManager.Application.Common;

namespace RpgManager.Application.Spells;

public interface ISpellService
{
    Task<PagedResponse<SpellResponse>> GetVisibleAsync(Guid userId, SpellFilters filters, CancellationToken cancellationToken);
    Task<ServiceResult<SpellResponse>> GetByIdAsync(Guid userId, Guid spellId, CancellationToken cancellationToken);
    Task<ServiceResult<SpellResponse>> CreateAsync(Guid userId, SpellRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<SpellResponse>> UpdateAsync(Guid userId, Guid spellId, SpellRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteAsync(Guid userId, Guid spellId, CancellationToken cancellationToken);
}

public interface ISpellImportService
{
    Task<SpellImportResponse> ImportOpen5eAsync(Guid userId, CancellationToken cancellationToken);
}
