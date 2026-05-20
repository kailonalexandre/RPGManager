namespace RpgManager.Domain.Entities;

public sealed class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string? AvatarUrl { get; set; }
    public string? TokenImageUrl { get; set; }
    public int TotalLevel { get; set; } = 1;
    public string Species { get; set; } = string.Empty;
    public Guid? RaceId { get; set; }
    public Race? Race { get; set; }
    public string MainClass { get; set; } = string.Empty;
    public Guid? ClassId { get; set; }
    public CharacterClass? Class { get; set; }
    public string Subclass { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public Guid? BackgroundId { get; set; }
    public Background? BackgroundOption { get; set; }
    public string Alignment { get; set; } = string.Empty;
    public int Experience { get; set; }
    public bool Inspiration { get; set; }
    public int ProficiencyBonus { get; set; } = 2;
    public int ArmorClass { get; set; } = 10;
    public int Initiative { get; set; }
    public int Speed { get; set; } = 9;
    public int MaxHitPoints { get; set; }
    public int CurrentHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }
    public string TotalHitDice { get; set; } = string.Empty;
    public string AvailableHitDice { get; set; } = string.Empty;
    public string PhysicalDescription { get; set; } = string.Empty;
    public string PersonalityTraits { get; set; } = string.Empty;
    public string Ideals { get; set; } = string.Empty;
    public string Bonds { get; set; } = string.Empty;
    public string Flaws { get; set; } = string.Empty;
    public string Backstory { get; set; } = string.Empty;
    public string QuickNotes { get; set; } = string.Empty;
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Charisma { get; set; } = 10;
    public bool StrengthSaveProficient { get; set; }
    public bool DexteritySaveProficient { get; set; }
    public bool ConstitutionSaveProficient { get; set; }
    public bool IntelligenceSaveProficient { get; set; }
    public bool WisdomSaveProficient { get; set; }
    public bool CharismaSaveProficient { get; set; }
    public int StrengthSaveCustomBonus { get; set; }
    public int DexteritySaveCustomBonus { get; set; }
    public int ConstitutionSaveCustomBonus { get; set; }
    public int IntelligenceSaveCustomBonus { get; set; }
    public int WisdomSaveCustomBonus { get; set; }
    public int CharismaSaveCustomBonus { get; set; }
    public int Copper { get; set; }
    public int Silver { get; set; }
    public int Electrum { get; set; }
    public int Gold { get; set; }
    public int Platinum { get; set; }
    public List<CharacterSkill> Skills { get; set; } = [];
    public List<CharacterAttack> Attacks { get; set; } = [];
    public List<CharacterCondition> Conditions { get; set; } = [];
    public List<CharacterNote> Notes { get; set; } = [];
    public List<CharacterInventoryItem> InventoryItems { get; set; } = [];
    public List<CharacterAsset> Assets { get; set; } = [];
    public List<CharacterSpell> Spells { get; set; } = [];
    public List<CharacterSpellSlot> SpellSlots { get; set; } = [];
    public List<CharacterFeature> Features { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
