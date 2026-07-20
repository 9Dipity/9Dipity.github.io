using InventorySyncDemo.Core.Models;
using InventorySyncDemo.Core.Services;
using Xunit;

namespace InventorySyncDemo.Tests;

public class StockServiceTests
{
    [Fact]
    public void TrySell_LastUnit_BringsStockToExactlyZero_AndFurtherSellsFail()
    {
        var service = new StockService();
        const string sku = "NFH-BOT-001"; // seeded with 3 units
        var startingStock = service.GetStock(sku);
        Assert.True(startingStock > 0);

        // Sell down to zero one unit at a time.
        for (var i = 0; i < startingStock; i++)
        {
            var sold = service.TrySell(sku, 1, Channel.RetailStoreA);
            Assert.True(sold);
        }

        Assert.Equal(0, service.GetStock(sku));

        // Further sells must fail and stock must never go negative.
        var extraSaleResult = service.TrySell(sku, 1, Channel.RetailStoreA);
        Assert.False(extraSaleResult);
        Assert.Equal(0, service.GetStock(sku));
    }

    [Fact]
    public void Sale_InOneChannel_IsVisibleViaGetStock_RegardlessOfWhichChannelSold()
    {
        var service = new StockService();
        const string sku = "NFH-JKT-001";
        var before = service.GetStock(sku);

        var sold = service.TrySell(sku, 2, Channel.LivestreamOnline);

        Assert.True(sold);
        // Shared pool: reading stock is not scoped to a channel, so it reflects the sale
        // regardless of which channel triggered it.
        Assert.Equal(before - 2, service.GetStock(sku));
        Assert.Equal(before - 2, service.GetAllStock()[sku]);
    }

    [Fact]
    public void TrySell_MoreThanAvailable_FailsWithoutPartialDeduction()
    {
        var service = new StockService();
        const string sku = "NFH-BOT-003"; // seeded with 5 units
        var before = service.GetStock(sku);

        var sold = service.TrySell(sku, before + 100, Channel.RetailStoreB);

        Assert.False(sold);
        Assert.Equal(before, service.GetStock(sku)); // unchanged - no partial deduction
    }

    [Fact]
    public void TrySell_UnknownSku_ReturnsFalse()
    {
        var service = new StockService();
        var sold = service.TrySell("NOT-A-REAL-SKU", 1, Channel.RetailStoreA);
        Assert.False(sold);
    }

    [Fact]
    public void TrySell_ZeroOrNegativeQuantity_ReturnsFalse()
    {
        var service = new StockService();
        const string sku = "NFH-JKT-001";
        var before = service.GetStock(sku);

        Assert.False(service.TrySell(sku, 0, Channel.RetailStoreA));
        Assert.False(service.TrySell(sku, -1, Channel.RetailStoreA));
        Assert.Equal(before, service.GetStock(sku));
    }

    [Fact]
    public void Reset_RestoresSeedStockAndClearsSalesHistory()
    {
        var service = new StockService();
        const string sku = "NFH-JKT-001";
        var seedStock = service.GetStock(sku);

        service.TrySell(sku, 1, Channel.RetailStoreA);
        Assert.NotEqual(seedStock, service.GetStock(sku));
        Assert.NotEmpty(service.RecentSales);

        service.Reset();

        Assert.Equal(seedStock, service.GetStock(sku));
        Assert.Empty(service.RecentSales);
    }

    [Fact]
    public void StockChanged_Event_FiresOnSuccessfulSale()
    {
        var service = new StockService();
        var fired = false;
        service.StockChanged += () => fired = true;

        service.TrySell("NFH-JKT-001", 1, Channel.RetailStoreA);

        Assert.True(fired);
    }

    [Fact]
    public void StockChanged_Event_DoesNotFireOnFailedSale()
    {
        var service = new StockService();
        var fired = false;
        service.StockChanged += () => fired = true;

        service.TrySell("NOT-A-REAL-SKU", 1, Channel.RetailStoreA);

        Assert.False(fired);
    }
}
