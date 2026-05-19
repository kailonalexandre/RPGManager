using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CharacterCondition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public ConditionType ConditionType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Notes { get; set; } = string.Empty;
}
