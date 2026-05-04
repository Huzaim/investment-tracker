namespace InvestmentTracker.Application.Assets;

public interface IAssetQueryService
{
    Task<IReadOnlyCollection<AssetResponse>> SearchAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<AssetResponse?> GetByTickerAsync(
        string ticker,
        CancellationToken cancellationToken = default);
}
