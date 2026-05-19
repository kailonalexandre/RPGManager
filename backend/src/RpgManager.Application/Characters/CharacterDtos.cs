namespace RpgManager.Application.Characters;

using RpgManager.Domain.Enums;

public sealed record CharacterRequest(
    Guid? CampaignId,
    string Name,
    string? Nickname,
    string? AvatarUrl,
    string? TokenImageUrl,
    int TotalLevel,
    string Species,
    string MainClass,
    string Subclass,
    string Background,
    string Alignment,
    int Experience,
    bool Inspiration,
    int ProficiencyBonus,
    int ArmorClass,
    int Initiative,
    int Speed,
    int MaxHitPoints,
    int CurrentHitPoints,
    int TemporaryHitPoints,
    string TotalHitDice,
    string AvailableHitDice,
    string PhysicalDescription,
    string PersonalityTraits,
    string Ideals,
    string Bonds,
    string Flaws,
    string Backstory,
    string QuickNotes);

public sealed record CharacterResponse(
    Guid Id,
    Guid UserId,
    Guid? CampaignId,
    string? CampaignName,
    string Name,
    string? Nickname,
    string? AvatarUrl,
    string? TokenImageUrl,
    int TotalLevel,
    string Species,
    string MainClass,
    string Subclass,
    string Background,
    string Alignment,
    int Experience,
    bool Inspiration,
    int ProficiencyBonus,
    int ArmorClass,
    int Initiative,
    int Speed,
    int MaxHitPoints,
    int CurrentHitPoints,
    int TemporaryHitPoints,
    string TotalHitDice,
    string AvailableHitDice,
    string PhysicalDescription,
    string PersonalityTraits,
    string Ideals,
    string Bonds,
    string Flaws,
    string Backstory,
    string QuickNotes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool CanEdit);

public sealed record CharacterSummaryResponse(
    Guid Id,
    Guid UserId,
    Guid? CampaignId,
    string? CampaignName,
    string Name,
    string? Nickname,
    string? AvatarUrl,
    int TotalLevel,
    string Species,
    string MainClass,
    string Subclass,
    int ArmorClass,
    int CurrentHitPoints,
    int MaxHitPoints,
    bool CanEdit);

public sealed record AbilityScoreRequest(
    int Strength,
    int Dexterity,
    int Constitution,
    int Intelligence,
    int Wisdom,
    int Charisma);

public sealed record AbilityScoreResponse(
    AbilityType Attribute,
    string Label,
    int Score,
    int Modifier);

public sealed record SavingThrowRequest(
    AbilityType Attribute,
    bool IsProficient,
    int CustomBonus);

public sealed record SavingThrowResponse(
    AbilityType Attribute,
    string Label,
    int Modifier,
    bool IsProficient,
    int CustomBonus,
    int FinalValue);

public sealed record CharacterSkillRequest(
    SkillType SkillType,
    bool IsProficient,
    bool IsExpertise,
    int CustomBonus);

public sealed record CharacterSkillResponse(
    Guid Id,
    SkillType SkillType,
    string Label,
    AbilityType BaseAttribute,
    string BaseAttributeLabel,
    bool IsProficient,
    bool IsExpertise,
    int CustomBonus,
    int FinalValue);

public sealed record CharacterCombatRequest(
    int ArmorClass,
    int Initiative,
    int Speed,
    int MaxHitPoints,
    int CurrentHitPoints,
    int TemporaryHitPoints,
    string TotalHitDice,
    string AvailableHitDice);

public sealed record CharacterCombatResponse(
    int ArmorClass,
    int Initiative,
    int Speed,
    int MaxHitPoints,
    int CurrentHitPoints,
    int TemporaryHitPoints,
    string TotalHitDice,
    string AvailableHitDice);

public sealed record CharacterAttackRequest(
    string Name,
    int AttackBonus,
    string Damage,
    string DamageType,
    string Range,
    AbilityType? UsesAttribute,
    string Notes);

public sealed record CharacterAttackResponse(
    Guid Id,
    string Name,
    int AttackBonus,
    string Damage,
    string DamageType,
    string Range,
    AbilityType? UsesAttribute,
    string? UsesAttributeLabel,
    string Notes);

public sealed record CharacterConditionRequest(
    ConditionType ConditionType,
    string Name,
    string Description,
    bool IsActive,
    string Notes);

public sealed record CharacterConditionResponse(
    Guid Id,
    ConditionType ConditionType,
    string Name,
    string Description,
    bool IsActive,
    string Notes);

public sealed record CharacterNoteRequest(
    string Title,
    string Content,
    string Category,
    string Tags,
    bool IsPrivate,
    bool IsVisibleToMaster);

public sealed record CharacterNoteResponse(
    Guid Id,
    Guid CharacterId,
    string Title,
    string Content,
    string Category,
    string Tags,
    bool IsPrivate,
    bool IsVisibleToMaster,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool CanEdit);

public sealed record CharacterInventoryItemRequest(
    string Name,
    string Description,
    int Quantity,
    decimal Weight,
    decimal Value,
    ItemType ItemType,
    bool Equipped,
    bool Attuned,
    string Notes);

public sealed record CharacterInventoryItemResponse(
    Guid Id,
    Guid CharacterId,
    string Name,
    string Description,
    int Quantity,
    decimal Weight,
    decimal Value,
    ItemType ItemType,
    string ItemTypeLabel,
    bool Equipped,
    bool Attuned,
    string Notes,
    decimal TotalWeight,
    bool CanEdit);

public sealed record CharacterCurrencyRequest(
    int Copper,
    int Silver,
    int Electrum,
    int Gold,
    int Platinum);

public sealed record CharacterCurrencyResponse(
    int Copper,
    int Silver,
    int Electrum,
    int Gold,
    int Platinum,
    bool CanEdit);

public sealed record CharacterAssetResponse(
    Guid Id,
    Guid CharacterId,
    string FileName,
    string FileUrl,
    string FileType,
    AssetType AssetType,
    DateTime UploadedAt,
    bool CanEdit);

public sealed record CharacterSpellRequest(
    Guid SpellId,
    bool IsKnown,
    bool IsPrepared,
    bool IsFavorite,
    string Notes);

public sealed record CharacterSpellUpdateRequest(
    bool IsKnown,
    bool IsPrepared,
    bool IsFavorite,
    string Notes);

public sealed record CharacterSpellResponse(
    Guid Id,
    Guid CharacterId,
    Guid SpellId,
    string Name,
    string EnglishName,
    int Level,
    string School,
    string CastingTime,
    string Range,
    string Components,
    string Material,
    string Duration,
    bool IsConcentration,
    bool IsRitual,
    string Description,
    string HigherLevelDescription,
    string AvailableClasses,
    string Source,
    bool IsHomebrew,
    bool IsKnown,
    bool IsPrepared,
    bool IsFavorite,
    string Notes,
    bool CanEdit);

public sealed record CharacterSpellSlotRequest(
    int SpellLevel,
    int TotalSlots,
    int UsedSlots);

public sealed record CharacterSpellSlotResponse(
    Guid Id,
    Guid CharacterId,
    int SpellLevel,
    int TotalSlots,
    int UsedSlots,
    bool CanEdit);

public sealed record CharacterFeatureRequest(
    Guid? FeatureId,
    string CustomName,
    string CustomDescription,
    int MaxUses,
    int CurrentUses,
    RecoveryType RecoveryType,
    string Notes);

public sealed record CharacterFeatureResponse(
    Guid Id,
    Guid CharacterId,
    Guid? FeatureId,
    string Name,
    FeatureType? Type,
    string? TypeLabel,
    string Description,
    string Source,
    string Prerequisites,
    bool IsHomebrew,
    string CustomName,
    string CustomDescription,
    int MaxUses,
    int CurrentUses,
    RecoveryType RecoveryType,
    string RecoveryTypeLabel,
    string Notes,
    bool CanEdit);

public sealed record CharacterRestRequest(
    bool RestoreHitPoints,
    bool RestoreHitDice);

public sealed record CharacterRestResponse(
    CharacterResponse Character,
    IReadOnlyList<CharacterFeatureResponse> Features,
    IReadOnlyList<CharacterSpellSlotResponse> SpellSlots);
