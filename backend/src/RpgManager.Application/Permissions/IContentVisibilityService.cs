namespace RpgManager.Application.Permissions;

public interface IContentVisibilityService
{
    Task<bool> CanViewContentAsync(Guid contentId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditContentAsync(Guid contentId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanViewSpellAsync(Guid spellId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditSpellAsync(Guid spellId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanViewFeatureAsync(Guid featureId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditFeatureAsync(Guid featureId, Guid userId, CancellationToken cancellationToken);
}
