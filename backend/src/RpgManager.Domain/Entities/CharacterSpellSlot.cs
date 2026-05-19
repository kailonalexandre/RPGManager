namespace RpgManager.Domain.Entities;

public sealed class CharacterSpellSlot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public int SpellLevel { get; set; }
    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }
}
