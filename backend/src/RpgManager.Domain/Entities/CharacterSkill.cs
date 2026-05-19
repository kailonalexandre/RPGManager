using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CharacterSkill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public SkillType SkillType { get; set; }
    public AbilityType BaseAttribute { get; set; }
    public bool IsProficient { get; set; }
    public bool IsExpertise { get; set; }
    public int CustomBonus { get; set; }
}
