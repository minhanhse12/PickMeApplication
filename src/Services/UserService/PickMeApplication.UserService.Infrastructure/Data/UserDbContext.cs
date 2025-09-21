using Microsoft.EntityFrameworkCore;
using PickMeApplication.UserService.Domain.Entities;

namespace PickMeApplication.UserService.Infrastructure.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Role)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.LastLoginAt);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            entity.HasIndex(e => e.Email)
                .IsUnique();
            
            entity.HasIndex(e => e.Username)
                .IsUnique();
        });
    }
}