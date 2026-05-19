using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class Feature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public FeatureType Type { get; set; } = FeatureType.Homebrew;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public bool IsHomebrew { get; set; } = true;
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public SpellVisibility Visibility { get; set; } = SpellVisibility.Private;
    public Guid? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
