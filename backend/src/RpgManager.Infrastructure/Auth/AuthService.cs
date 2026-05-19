using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Auth;
using RpgManager.Application.Common;
using RpgManager.Domain.Entities;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Auth;

public sealed class AuthService(AppDbContext dbContext, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult<AuthResponse>.Failure("Nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return ServiceResult<AuthResponse>.Failure("E-mail é obrigatório.");
        }

        if (request.Password.Length < 8)
        {
            return ServiceResult<AuthResponse>.Failure("Senha deve ter pelo menos 8 caracteres.");
        }

        var emailExists = await dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return ServiceResult<AuthResponse>.Failure("E-mail já cadastrado.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Profile = request.Profile
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<AuthResponse>.Success(CreateAuthResponse(user));
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await dbContext.Users
            .SingleOrDefaultAsync(item => item.Email == normalizedEmail, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponse>.Failure("E-mail ou senha inválidos.");
        }

        return ServiceResult<AuthResponse>.Success(CreateAuthResponse(user));
    }

    public async Task<UserResponse?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([userId], cancellationToken);
        return user is null ? null : ToResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var token = jwtTokenService.CreateToken(user);
        return new AuthResponse(token, ToResponse(user));
    }

    private static UserResponse ToResponse(User user)
        => new(user.Id, user.Name, user.Email, user.Profile, user.AvatarUrl, user.CreatedAt);

    private static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();
}
