using InventorySyncDemo.Core.Models;
using InventorySyncDemo.Core.Services;
using Xunit;

namespace InventorySyncDemo.Tests;

public class PriceNormalizationServiceTests
{
    [Fact]
    public void Normalize_MatchesBySkuAndComputesDirectionAgainstCurrentCatalogPrice()
    {
        var stockService = new StockService();
        var normalizer = new PriceNormalizationService(stockService);

        // NFH-JKT-001 catalog price is 189.00 (see SeedData).
        var entries = new List<SupplierPriceListEntry>
        {
            new("NFH-JKT-001", "Nordic Textiles", 199.00m), // up
            new("NFH-SWT-004", "Nordic Textiles", 59.00m),  // same (catalog price is 59.00)
        };

        var result = normalizer.Normalize(entries);

        Assert.Equal(2, result.Count);

        var up = result.Single(r => r.Sku == "NFH-JKT-001");
        Assert.Equal(189.00m, up.PreviousPrice);
        Assert.Equal(199.00m, up.NewPrice);
        Assert.Equal(PriceDirection.Up, up.Direction);

        var same = result.Single(r => r.Sku == "NFH-SWT-004");
        Assert.Equal(PriceDirection.Same, same.Direction);
    }

    [Fact]
    public void Normalize_SkipsEntriesWithUnknownSku()
    {
        var stockService = new StockService();
        var normalizer = new PriceNormalizationService(stockService);

        var entries = new List<SupplierPriceListEntry>
        {
            new("NOT-A-REAL-SKU", "Nordic Textiles", 50.00m)
        };

        var result = normalizer.Normalize(entries);

        Assert.Empty(result);
    }
}
