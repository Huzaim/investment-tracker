using InvestmentTracker.Infrastructure.Persistence.Users;
using InvestmentTracker.Domain.Assets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.Infrastructure.Persistence;

public sealed class InvestmentTrackerDbContext(DbContextOptions<InvestmentTrackerDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Asset> Assets => Set<Asset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvestmentTrackerDbContext).Assembly);
    }
}
