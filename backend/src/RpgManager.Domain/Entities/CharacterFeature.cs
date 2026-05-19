using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CharacterFeature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public Guid? FeatureId { get; set; }
    public Feature? Feature { get; set; }
    public string CustomName { get; set; } = string.Empty;
    public string CustomDescription { get; set; } = string.Empty;
    public int MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public RecoveryType RecoveryType { get; set; } = RecoveryType.Manual;
    public string Notes { get; set; } = string.Empty;
}
