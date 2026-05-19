namespace RpgManager.Domain.Entities;

public sealed class CharacterSpell
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public Guid SpellId { get; set; }
    public Spell Spell { get; set; } = null!;
    public bool IsKnown { get; set; } = true;
    public bool IsPrepared { get; set; }
    public bool IsFavorite { get; set; }
    public string Notes { get; set; } = string.Empty;
}
