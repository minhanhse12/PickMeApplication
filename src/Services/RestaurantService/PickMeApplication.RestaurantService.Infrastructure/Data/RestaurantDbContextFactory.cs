using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PickMeApplication.RestaurantService.Infrastructure.Data;

public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
{
    public RestaurantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();
        
        // Use the connection string for design-time operations
        optionsBuilder.UseMySql(
            "Server=localhost;Database=PickMeRestaurantService_Dev;Uid=root;Pwd=rootpassword;",
            new MySqlServerVersion(new Version(8, 0, 21))
        );

        return new RestaurantDbContext(optionsBuilder.Options);
    }
}