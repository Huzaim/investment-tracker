using InvestmentTracker.Domain.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentTracker.Infrastructure.Persistence.Configurations;

internal sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(asset => asset.Id);

        builder.Property(asset => asset.Ticker)
            .HasMaxLength(16)
            .IsRequired();

        builder.HasIndex(asset => asset.Ticker)
            .IsUnique();

        builder.Property(asset => asset.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(asset => asset.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(asset => asset.Exchange)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasData(
            new Asset(
                Guid.Parse("0f85a8e7-8c6d-4f9f-8f9b-3df9274a0f1a"),
                "AAPL",
                "Apple Inc.",
                AssetType.Stock,
                "NASDAQ"),
            new Asset(
                Guid.Parse("6e8e6a5f-7e53-4cbb-80a5-b5a44c9e2c28"),
                "MSFT",
                "Microsoft Corporation",
                AssetType.Stock,
                "NASDAQ"),
            new Asset(
                Guid.Parse("a1fb97da-62ed-41f6-a5e8-c15b7d2060bf"),
                "VOO",
                "Vanguard S&P 500 ETF",
                AssetType.Etf,
                "NYSEARCA"),
            new Asset(
                Guid.Parse("ab42c1c4-690f-4ce4-927e-ea967dfd69cf"),
                "BTC",
                "Bitcoin",
                AssetType.Crypto,
                "CRYPTO"),
            new Asset(
                Guid.Parse("5c25df89-01e0-47d7-b01e-cc4c69d87f04"),
                "ETH",
                "Ethereum",
                AssetType.Crypto,
                "CRYPTO"));
    }
}
