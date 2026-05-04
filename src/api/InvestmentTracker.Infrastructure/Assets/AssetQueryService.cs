using InvestmentTracker.Application.Assets;
using InvestmentTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.Infrastructure.Assets;

internal sealed class AssetQueryService(InvestmentTrackerDbContext dbContext) : IAssetQueryService
{
    public async Task<IReadOnlyCollection<AssetResponse>> SearchAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Assets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToUpperInvariant();

            query = query.Where(asset =>
                asset.Ticker.Contains(normalizedSearch) ||
                asset.Name.ToUpper().Contains(normalizedSearch));
        }

        return await query
            .OrderBy(asset => asset.Ticker)
            .Select(asset => new AssetResponse(
                asset.Id,
                asset.Ticker,
                asset.Name,
                asset.Type,
                asset.Exchange))
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetResponse?> GetByTickerAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return null;
        }

        var normalizedTicker = ticker.Trim().ToUpperInvariant();

        return await dbContext.Assets
            .AsNoTracking()
            .Where(asset => asset.Ticker == normalizedTicker)
            .Select(asset => new AssetResponse(
                asset.Id,
                asset.Ticker,
                asset.Name,
                asset.Type,
                asset.Exchange))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
