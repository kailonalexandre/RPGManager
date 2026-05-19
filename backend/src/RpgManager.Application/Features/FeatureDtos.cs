using RpgManager.Domain.Enums;

namespace RpgManager.Application.Features;

public sealed record FeatureRequest(
    string Name,
    FeatureType Type,
    string Description,
    string Source,
    string Prerequisites,
    bool IsHomebrew,
    SpellVisibility Visibility,
    Guid? CampaignId);

public sealed record FeatureFilters(
    string? Name,
    FeatureType? Type,
    string? Source,
    bool? IsHomebrew,
    SpellVisibility? Visibility,
    int Page = 1,
    int PageSize = 20);

public sealed record FeatureResponse(
    Guid Id,
    string Name,
    FeatureType Type,
    string Description,
    string Source,
    string Prerequisites,
    bool IsHomebrew,
    Guid CreatedByUserId,
    string CreatedByUserName,
    SpellVisibility Visibility,
    Guid? CampaignId,
    string? CampaignName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool CanEdit);
