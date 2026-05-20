using RpgManager.Domain.Enums;

namespace RpgManager.Application.Npcs;

public sealed record NpcQuery(
    string? Search,
    string? Tag,
    string? Location,
    string? Faction,
    bool? IsImportant,
    bool? IsAlive,
    Visibility? Visibility);

public sealed record NpcRequest(
    string Name,
    string Alias,
    string Race,
    string Occupation,
    string Location,
    string Faction,
    string Personality,
    string Appearance,
    string Motivation,
    string Secrets,
    string Notes,
    string StatBlockJson,
    string Tags,
    bool IsImportant,
    bool IsAlive,
    Visibility Visibility);

public sealed record NpcResponse(
    Guid Id,
    Guid CampaignId,
    Guid CreatedByUserId,
    string CreatedByUserName,
    string Name,
    string Alias,
    string Race,
    string Occupation,
    string Location,
    string Faction,
    string Personality,
    string Appearance,
    string Motivation,
    string? Secrets,
    string Notes,
    string StatBlockJson,
    string Tags,
    bool IsImportant,
    bool IsAlive,
    Visibility Visibility,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool CanEdit);
