using RpgManager.Domain.Entities;

namespace RpgManager.Application.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
