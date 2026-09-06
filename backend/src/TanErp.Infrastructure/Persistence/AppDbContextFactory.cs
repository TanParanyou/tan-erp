using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TanErp.Infrastructure.Configuration;

namespace TanErp.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = args.FirstOrDefault(a => a.Contains("Host="))
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__Database");

        connectionString = RequiredConfiguration.Require(
            connectionString,
            "ConnectionStrings__Database");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
