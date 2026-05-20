using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CampaignNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid OwnerUserId { get; set; }
    public User OwnerUser { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public Visibility Visibility { get; set; } = Visibility.Private;
    public string? LinkedEntityType { get; set; }
    public Guid? LinkedEntityId { get; set; }
    public ExternalProvider ExternalProvider { get; set; } = ExternalProvider.None;
    public string? ExternalId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
