using RpgManager.Domain.Enums;

namespace RpgManager.Application.CampaignNotes;

public sealed record CampaignNoteQuery(
    string? Search,
    string? Tag,
    Visibility? Visibility,
    string? LinkedEntityType,
    Guid? LinkedEntityId);

public sealed record CampaignNoteRequest(
    string Title,
    string ContentMarkdown,
    string Tags,
    Visibility? Visibility,
    string? LinkedEntityType,
    Guid? LinkedEntityId,
    ExternalProvider ExternalProvider,
    string? ExternalId);

public sealed record CampaignNoteResponse(
    Guid Id,
    Guid CampaignId,
    Guid OwnerUserId,
    string OwnerUserName,
    string Title,
    string ContentMarkdown,
    string Tags,
    Visibility Visibility,
    string? LinkedEntityType,
    Guid? LinkedEntityId,
    ExternalProvider ExternalProvider,
    string? ExternalId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool CanEdit);
