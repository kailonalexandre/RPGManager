using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CharacterInventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal Weight { get; set; }
    public decimal Value { get; set; }
    public ItemType ItemType { get; set; } = ItemType.Other;
    public bool Equipped { get; set; }
    public bool Attuned { get; set; }
    public string Notes { get; set; } = string.Empty;
}
