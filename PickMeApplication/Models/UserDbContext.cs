using Microsoft.EntityFrameworkCore;
namespace PickMeApplication.Models

{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                      .HasColumnType("char(36)");

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(u => u.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(255);
                
                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.CreatedAt)
                      .IsRequired();

                entity.Property(u => u.IsActive)
                      .IsRequired()
                      .HasDefaultValue(true);
            });
        }
    }
}