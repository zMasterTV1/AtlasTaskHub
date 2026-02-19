using Atlas.Domain.Entities;

namespace Atlas.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Find user that owns an active refresh token with the given hash.</summary>
    Task<User?> FindByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct);

    void Add(User user);
}
