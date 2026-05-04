using InvestmentTracker.Domain.Assets;

namespace InvestmentTracker.Application.Assets;

public sealed record AssetResponse(
    Guid Id,
    string Ticker,
    string Name,
    AssetType Type,
    string Exchange);
