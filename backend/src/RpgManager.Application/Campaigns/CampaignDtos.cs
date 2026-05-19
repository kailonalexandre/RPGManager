using RpgManager.Domain.Enums;

namespace RpgManager.Application.Campaigns;

public sealed record CampaignRequest(
    string Name,
    string Description,
    string System,
    string? CoverImageUrl);

public sealed record JoinCampaignRequest(string InviteCode);

public sealed record CampaignMemberResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string Email,
    CampaignRole Role,
    DateTime JoinedAt);

public sealed record CampaignResponse(
    Guid Id,
    string Name,
    string Description,
    string System,
    string? CoverImageUrl,
    string InviteCode,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    CampaignRole CurrentUserRole,
    IReadOnlyList<CampaignMemberResponse> Members);

public sealed record CampaignSummaryResponse(
    Guid Id,
    string Name,
    string Description,
    string System,
    string? CoverImageUrl,
    DateTime CreatedAt,
    CampaignRole CurrentUserRole,
    int MemberCount);

public sealed record CampaignCharacterSummaryResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string Name,
    string MainClass,
    int TotalLevel,
    int CurrentHitPoints,
    int MaxHitPoints,
    int ArmorClass,
    int PassivePerception);

public sealed record CampaignMasterNoteResponse(
    Guid Id,
    Guid CharacterId,
    string CharacterName,
    string Title,
    string Content,
    string Category,
    string Tags,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CampaignMasterDashboardResponse(
    Guid CampaignId,
    string CampaignName,
    IReadOnlyList<CampaignMemberResponse> Members,
    IReadOnlyList<CampaignCharacterSummaryResponse> Characters,
    IReadOnlyList<CampaignMasterNoteResponse> VisibleNotes);
