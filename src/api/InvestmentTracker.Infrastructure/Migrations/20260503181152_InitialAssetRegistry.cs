using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InvestmentTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAssetRegistry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ticker = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Exchange = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "Exchange", "Name", "Ticker", "Type" },
                values: new object[,]
                {
                    { new Guid("0f85a8e7-8c6d-4f9f-8f9b-3df9274a0f1a"), "NASDAQ", "Apple Inc.", "AAPL", "Stock" },
                    { new Guid("5c25df89-01e0-47d7-b01e-cc4c69d87f04"), "CRYPTO", "Ethereum", "ETH", "Crypto" },
                    { new Guid("6e8e6a5f-7e53-4cbb-80a5-b5a44c9e2c28"), "NASDAQ", "Microsoft Corporation", "MSFT", "Stock" },
                    { new Guid("a1fb97da-62ed-41f6-a5e8-c15b7d2060bf"), "NYSEARCA", "Vanguard S&P 500 ETF", "VOO", "Etf" },
                    { new Guid("ab42c1c4-690f-4ce4-927e-ea967dfd69cf"), "CRYPTO", "Bitcoin", "BTC", "Crypto" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Ticker",
                table: "Assets",
                column: "Ticker",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
