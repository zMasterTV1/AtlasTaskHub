using Atlas.Domain.Entities;

namespace Atlas.Application.Abstractions.Security;

public interface IJwtTokenService
{
    (string token, DateTimeOffset expiresAt) CreateAccessToken(User user);
    (string rawToken, string tokenHash, DateTimeOffset expiresAt) CreateRefreshToken();
}
