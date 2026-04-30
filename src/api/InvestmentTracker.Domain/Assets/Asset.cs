namespace InvestmentTracker.Domain.Assets;

public sealed class Asset
{
    public Asset(Guid id, string ticker, string name, AssetType type, string exchange)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Asset ID cannot be empty.", nameof(id));
        }

        Id = id;
        Ticker = NormalizeRequired(ticker, nameof(ticker)).ToUpperInvariant();
        Name = NormalizeRequired(name, nameof(name));
        Type = type;
        Exchange = NormalizeRequired(exchange, nameof(exchange)).ToUpperInvariant();
    }

    public Guid Id { get; }

    public string Ticker { get; }

    public string Name { get; }

    public AssetType Type { get; }

    public string Exchange { get; }

    private static string NormalizeRequired(string value, string parameterName)
    {
        return string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value cannot be empty.", parameterName) : value.Trim();
    }
}
