using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InvestmentTracker.Infrastructure.Persistence;

public sealed class InvestmentTrackerDbContextFactory : IDesignTimeDbContextFactory<InvestmentTrackerDbContext>
{
    public InvestmentTrackerDbContext CreateDbContext(string[] args)
    {
        var basePath = FindSolutionRoot();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(basePath, "src", "api", "InvestmentTracker.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("InvestmentTrackerDb")
            ?? throw new InvalidOperationException("Connection string 'InvestmentTrackerDb' is not configured.");

        var optionsBuilder = new DbContextOptionsBuilder<InvestmentTrackerDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new InvestmentTrackerDbContext(optionsBuilder.Options);
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "investment-tracker.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Could not locate the solution root.");
    }
}
