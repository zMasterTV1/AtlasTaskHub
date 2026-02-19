using Atlas.Application.Abstractions.Security;
using Atlas.Application.Abstractions.Time;
using Atlas.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Atlas.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;
    private readonly IClock _clock;

    public JwtTokenService(JwtOptions opt, IClock clock)
    {
        _opt = opt;
        _clock = clock;
    }

    public (string token, DateTimeOffset expiresAt) CreateAccessToken(User user)
    {
        var now = _clock.UtcNow;
        var expires = now.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("uid", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public (string rawToken, string tokenHash, DateTimeOffset expiresAt) CreateRefreshToken()
    {
        var rawBytes = RandomNumberGenerator.GetBytes(64);
        var raw = Base64UrlEncoder.Encode(rawBytes); // URL safe

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        var hashHex = Convert.ToHexString(hash);

        var expires = _clock.UtcNow.AddDays(_opt.RefreshTokenDays);
        return (raw, hashHex, expires);
    }
}
