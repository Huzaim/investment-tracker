using InvestmentTracker.Domain.Assets;

namespace InvestmentTracker.Tests.Domain.Assets;

public sealed class AssetTests
{
    [Fact]
    public void Constructor_NormalizesTickerAndExchange()
    {
        var asset = new Asset(
            Guid.NewGuid(),
            " aapl ",
            " Apple Inc. ",
            AssetType.Stock,
            " nasdaq ");

        Assert.Equal("AAPL", asset.Ticker);
        Assert.Equal("Apple Inc.", asset.Name);
        Assert.Equal("NASDAQ", asset.Exchange);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ThrowsWhenTickerIsEmpty(string ticker)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Asset(Guid.NewGuid(), ticker, "Apple Inc.", AssetType.Stock, "NASDAQ"));

        Assert.Equal("ticker", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ThrowsWhenNameIsEmpty(string name)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Asset(Guid.NewGuid(), "AAPL", name, AssetType.Stock, "NASDAQ"));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Constructor_ThrowsWhenIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Asset(Guid.Empty, "AAPL", "Apple Inc.", AssetType.Stock, "NASDAQ"));

        Assert.Equal("id", exception.ParamName);
    }
}
