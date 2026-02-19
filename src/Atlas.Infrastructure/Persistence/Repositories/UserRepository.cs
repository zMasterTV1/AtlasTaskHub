using Atlas.Application.Abstractions.Persistence;
using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AtlasDbContext _db;

    public UserRepository(AtlasDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct)
        => _db.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> FindByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct)
        => _db.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.TokenHash == refreshTokenHash), ct);

    public void Add(User user) => _db.Users.Add(user);
}
