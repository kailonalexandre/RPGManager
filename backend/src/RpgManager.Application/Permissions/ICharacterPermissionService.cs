namespace RpgManager.Application.Permissions;

public interface ICharacterPermissionService
{
    Task<bool> CanViewCharacterAsync(Guid characterId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditCharacterAsync(Guid characterId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanDeleteCharacterAsync(Guid characterId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanViewNoteAsync(Guid noteId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditNoteAsync(Guid noteId, Guid userId, CancellationToken cancellationToken);
}
