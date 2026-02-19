using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Persistence;

public sealed class AtlasDbContext : DbContext
{
    public AtlasDbContext(DbContextOptions<AtlasDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).HasMaxLength(320).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            b.Property(x => x.PasswordSalt).HasMaxLength(256).IsRequired();
            b.Property(x => x.Role).HasMaxLength(32).IsRequired();

            b.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            b.HasMany(x => x.Projects)
                .WithOne(x => x.Owner)
                .HasForeignKey(x => x.OwnerId);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshTokens");
            b.HasKey(x => x.Id);
            b.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            b.HasIndex(x => x.TokenHash).IsUnique();
        });

        modelBuilder.Entity<Project>(b =>
        {
            b.ToTable("Projects");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(2000);

            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasMany(x => x.Tasks)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId);
        });

        modelBuilder.Entity<TaskItem>(b =>
        {
            b.ToTable("Tasks");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).HasMaxLength(250).IsRequired();
            b.Property(x => x.Description).HasMaxLength(4000);

            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.RowVersion).IsRowVersion();

            b.HasIndex(x => new { x.ProjectId, x.Status });
        });
    }
}
