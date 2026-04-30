using InvestmentTracker.Domain.Assets;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.Infrastructure.Persistence;

public sealed class InvestmentTrackerDbContext(DbContextOptions<InvestmentTrackerDbContext> options)
    : DbContext(options)
{
    public DbSet<Asset> Assets => Set<Asset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvestmentTrackerDbContext).Assembly);
    }
}
