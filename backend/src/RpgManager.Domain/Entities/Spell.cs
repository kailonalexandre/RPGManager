using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class Spell
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public int Level { get; set; }
    public string School { get; set; } = string.Empty;
    public string CastingTime { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Components { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public bool IsConcentration { get; set; }
    public bool IsRitual { get; set; }
    public string Description { get; set; } = string.Empty;
    public string HigherLevelDescription { get; set; } = string.Empty;
    public string AvailableClasses { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool IsHomebrew { get; set; } = true;
    public string? ExternalSource { get; set; }
    public string? ExternalId { get; set; }
    public string? Slug { get; set; }
    public string? RulesVersion { get; set; }
    public bool IsImported { get; set; }
    public bool IsSrd { get; set; }
    public string Language { get; set; } = "pt-BR";
    public bool TranslationMissing { get; set; }
    public DateTime? ImportedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public SpellVisibility Visibility { get; set; } = SpellVisibility.Private;
    public Guid? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
