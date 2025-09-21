using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PickMeApplication.UserService.Infrastructure.Data;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        
        // Use connection string for design time
        var connectionString = "Server=localhost;Port=3306;Database=PickMeUserService_Dev;User=root;Password=rootpassword;";
        
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 0, 1)));
        
        return new UserDbContext(optionsBuilder.Options);
    }
}