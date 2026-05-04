using InvestmentTracker.Application.Assets;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.Api.Controllers;

[ApiController]
[Route("assets")]
public sealed class AssetsController(IAssetQueryService assetQueryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<AssetResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AssetResponse>>> Search(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var assets = await assetQueryService.SearchAsync(search, cancellationToken);

        return Ok(assets);
    }

    [HttpGet("{ticker}")]
    [ProducesResponseType<AssetResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssetResponse>> GetByTicker(
        string ticker,
        CancellationToken cancellationToken)
    {
        var asset = await assetQueryService.GetByTickerAsync(ticker, cancellationToken);

        return asset is null
            ? NotFound()
            : Ok(asset);
    }
}
