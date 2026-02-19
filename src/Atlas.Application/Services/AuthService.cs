using Atlas.Application.Abstractions.Persistence;
using Atlas.Application.Abstractions.Security;
using Atlas.Application.Abstractions.Time;
using Atlas.Application.Contracts.Auth;
using Atlas.Domain.Entities;

namespace Atlas.Application.Services;

public sealed class AuthService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokens;
    private readonly IClock _clock;

    public AuthService(IUserRepository users, IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtTokenService tokens, IClock clock)
    {
        _users = users;
        _uow = uow;
        _passwordHasher = passwordHasher;
        _tokens = tokens;
        _clock = clock;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 8)
            throw new InvalidOperationException("Invalid email/password.");

        var existing = await _users.FindByEmailAsync(email, ct);
        if (existing is not null) throw new InvalidOperationException("Email already registered.");

        var (hash, salt) = _passwordHasher.HashPassword(req.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = "User",
            CreatedAt = _clock.UtcNow
        };

        _users.Add(user);
        await _uow.SaveChangesAsync(ct);

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var user = await _users.FindByEmailAsync(email, ct) ?? throw new InvalidOperationException("Invalid credentials.");

        if (!_passwordHasher.VerifyPassword(req.Password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidOperationException("Invalid credentials.");

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest req, CancellationToken ct)
    {
        var tokenRaw = req.RefreshToken?.Trim();
        if (string.IsNullOrWhiteSpace(tokenRaw)) throw new InvalidOperationException("Missing refresh token.");

        // Hash provided token and compare with stored hashes
        var tokenHash = SecurityTokenHashing.Sha256(tokenRaw);

        var user = await _users.FindByRefreshTokenHashAsync(tokenHash, ct)
                   ?? throw new InvalidOperationException("Invalid refresh token.");

        var stored = user.RefreshTokens.Single(t => t.TokenHash == tokenHash);
        if (!stored.IsActive) throw new InvalidOperationException("Refresh token expired or revoked.");

        // Rotate refresh token
        stored.RevokedAt = _clock.UtcNow;

        await _uow.SaveChangesAsync(ct);
        return await IssueTokensAsync(user, ct);
    }

    public async Task LogoutAsync(Guid userId, string refreshToken, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct) ?? throw new InvalidOperationException("User not found.");
        var hash = SecurityTokenHashing.Sha256(refreshToken);

        var token = user.RefreshTokens.SingleOrDefault(t => t.TokenHash == hash);
        if (token is null) return;

        token.RevokedAt = _clock.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
    {
        var (accessToken, accessExp) = _tokens.CreateAccessToken(user);
        var (refreshRaw, refreshHash, refreshExp) = _tokens.CreateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            ExpiresAt = refreshExp,
            CreatedAt = _clock.UtcNow
        });

        await _uow.SaveChangesAsync(ct);

        return new AuthResponse(accessToken, accessExp, refreshRaw, refreshExp, user.Email, user.Role);
    }
}

/// <summary>Helper for hashing secrets (refresh tokens).</summary>
internal static class SecurityTokenHashing
{
    public static string Sha256(string input)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}
