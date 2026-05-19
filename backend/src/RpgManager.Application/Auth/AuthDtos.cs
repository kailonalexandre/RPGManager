using RpgManager.Domain.Enums;

namespace RpgManager.Application.Auth;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password,
    UserProfile Profile = UserProfile.Player);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(string Token, UserResponse User);

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    UserProfile Profile,
    string? AvatarUrl,
    DateTime CreatedAt);
