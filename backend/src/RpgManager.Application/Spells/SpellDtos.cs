using RpgManager.Domain.Enums;

namespace RpgManager.Application.Spells;

public sealed record SpellRequest(
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
    SpellVisibility Visibility,
    Guid? CampaignId);

public sealed record SpellFilters(
    string? Name,
    int? Level,
    string? School,
    string? Class,
    bool? IsConcentration,
    bool? IsRitual,
    string? Source,
    bool? IsHomebrew,
    SpellVisibility? Visibility,
    int Page = 1,
    int PageSize = 20);

public sealed record SpellResponse(
    Guid Id,
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
    Guid CreatedByUserId,
    string CreatedByUserName,
    SpellVisibility Visibility,
    Guid? CampaignId,
    string? CampaignName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool CanEdit);

public sealed record SpellImportResponse(
    int Created,
    int Updated,
    int Skipped,
    IReadOnlyList<string> Errors);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
