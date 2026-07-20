using InventorySyncDemo.Core.Models;
using InventorySyncDemo.Core.Parsers;
using Xunit;

namespace InventorySyncDemo.Tests;

public class PriceListParserTests
{
    [Fact]
    public void NordicTextilesParser_ParsesCommaDelimitedCsv()
    {
        var csv = "Sku,ProductName,PriceEUR\n" +
                  "NFH-JKT-001,Aurora Down Jacket,199.00\n" +
                  "NFH-SWT-001,Lambswool Crew Sweater,84.50\n";

        var parser = new SupplierNordicTextilesParser();
        var entries = parser.Parse(csv);

        Assert.Equal(2, entries.Count);

        var first = entries[0];
        Assert.Equal("NFH-JKT-001", first.SkuOrName);
        Assert.Equal("Nordic Textiles", first.Supplier);
        Assert.Equal(199.00m, first.NewPrice);

        var second = entries[1];
        Assert.Equal("NFH-SWT-001", second.SkuOrName);
        Assert.Equal(84.50m, second.NewPrice);
    }

    [Fact]
    public void BalticGoodsParser_ParsesSemicolonDelimitedCsvWithDecimalComma()
    {
        var csv = "Article Code;Item Description;Unit Price\n" +
                  "NFH-SWT-002;Cable Knit Cardigan;92,50\n" +
                  "NFH-BOT-002;Birch Chelsea Boots;145,00\n";

        var parser = new SupplierBalticGoodsParser();
        var entries = parser.Parse(csv);

        Assert.Equal(2, entries.Count);

        var first = entries[0];
        Assert.Equal("NFH-SWT-002", first.SkuOrName);
        Assert.Equal("Baltic Goods", first.Supplier);
        Assert.Equal(92.50m, first.NewPrice);

        var second = entries[1];
        Assert.Equal("NFH-BOT-002", second.SkuOrName);
        Assert.Equal(145.00m, second.NewPrice);
    }

    [Fact]
    public void ScandiHomeParser_ParsesTabDelimitedFileWithFlexibleColumnOrderAndCurrencySuffix()
    {
        var tsv = "Item Code\tCategory\tList Price\tCurrency\n" +
                  "NFH-BOT-003\tBoots\t169.00 EUR\tEUR\n" +
                  "NFH-ACC-002\tAccessories\t27.00 EUR\tEUR\n";

        var parser = new SupplierScandiHomeParser();
        var entries = parser.Parse(tsv);

        Assert.Equal(2, entries.Count);

        var first = entries[0];
        Assert.Equal("NFH-BOT-003", first.SkuOrName);
        Assert.Equal("Scandi Home", first.Supplier);
        Assert.Equal(169.00m, first.NewPrice);

        var second = entries[1];
        Assert.Equal("NFH-ACC-002", second.SkuOrName);
        Assert.Equal(27.00m, second.NewPrice);
    }

    [Theory]
    [InlineData(100.00, 120.00, PriceDirection.Up)]
    [InlineData(100.00, 80.00, PriceDirection.Down)]
    [InlineData(100.00, 100.00, PriceDirection.Same)]
    public void ComputeDirection_ReturnsExpectedDirection(decimal previous, decimal current, PriceDirection expected)
    {
        var direction = NormalizedPriceChange.ComputeDirection(previous, current);
        Assert.Equal(expected, direction);
    }
}
