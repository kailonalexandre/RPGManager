namespace RpgManager.Application.CharacterOptions;

public sealed record RaceRequest(string Name, string Description, string Source, bool IsHomebrew);

public sealed record RaceResponse(
    Guid Id,
    string Name,
    string Description,
    string Source,
    bool IsHomebrew,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CharacterClassRequest(string Name, int HitDie, string Description, string Source, bool IsHomebrew);

public sealed record CharacterClassResponse(
    Guid Id,
    string Name,
    int HitDie,
    string Description,
    string Source,
    bool IsHomebrew,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record BackgroundRequest(string Name, string Description, string Source, bool IsHomebrew);

public sealed record BackgroundResponse(
    Guid Id,
    string Name,
    string Description,
    string Source,
    bool IsHomebrew,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
