using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class Npc
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public string Personality { get; set; } = string.Empty;
    public string Appearance { get; set; } = string.Empty;
    public string Motivation { get; set; } = string.Empty;
    public string Secrets { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string StatBlockJson { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public bool IsImportant { get; set; }
    public bool IsAlive { get; set; } = true;
    public Visibility Visibility { get; set; } = Visibility.MasterOnly;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
