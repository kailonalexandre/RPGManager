using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CharacterAttack
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int AttackBonus { get; set; }
    public string Damage { get; set; } = string.Empty;
    public string DamageType { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public AbilityType? UsesAttribute { get; set; }
    public string Notes { get; set; } = string.Empty;
}
