using Microsoft.EntityFrameworkCore;
using PickMeApplication.RestaurantService.Domain.Entities;

namespace PickMeApplication.RestaurantService.Infrastructure.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantLocation> RestaurantLocations { get; set; }
    public DbSet<BusinessHours> BusinessHours { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Restaurant Configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DeliveryFeePerKm).HasPrecision(10, 2);
            entity.Property(e => e.MinimumOrderAmount).HasPrecision(10, 2);
            entity.Property(e => e.CuisineType).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();

            // Indexes
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CuisineType);
            entity.HasIndex(e => e.IsAcceptingOrders);
            entity.HasIndex(e => new { e.Status, e.IsAcceptingOrders });

            // Relationships
            entity.HasOne(e => e.Location)
                  .WithOne(l => l.Restaurant)
                  .HasForeignKey<RestaurantLocation>(l => l.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BusinessHours)
                  .WithOne(bh => bh.Restaurant)
                  .HasForeignKey(bh => bh.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // RestaurantLocation Configuration
        modelBuilder.Entity<RestaurantLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.District).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ward).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);

            // Indexes for location-based queries
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.District);
        });

        // BusinessHours Configuration
        modelBuilder.Entity<BusinessHours>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Notes).HasMaxLength(200);

            // Indexes
            entity.HasIndex(e => new { e.RestaurantId, e.DayOfWeek }).IsUnique();
            entity.HasIndex(e => e.Status);

            // Constraints
            entity.HasCheckConstraint("CK_BusinessHours_DayOfWeek", "[DayOfWeek] >= 0 AND [DayOfWeek] <= 6");
        });
    }
}