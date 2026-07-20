using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Services;

/// <summary>
/// The single shared stock pool for the whole business. All channels read and write
/// through this same service instance, so a sale made in one channel is instantly
/// visible to every other channel watching it. Register as a singleton.
/// </summary>
public interface IStockService
{
    /// <summary>Raised any time stock changes (sale, reset, etc). Subscribers should call StateHasChanged.</summary>
    event Action? StockChanged;

    /// <summary>
    /// The SKU affected by the most recent successful sale (null after a reset). UI
    /// components read this right after <see cref="StockChanged"/> fires to decide which
    /// row to briefly highlight - this is what makes the "same SKU flashes in every
    /// channel panel at once" moment work without any extra plumbing.
    /// </summary>
    string? LastChangedSku { get; }

    IReadOnlyList<Product> Catalog { get; }

    /// <summary>Current quantity on hand for a SKU (0 if unknown).</summary>
    int GetStock(string sku);

    /// <summary>All current stock levels, keyed by SKU.</summary>
    IReadOnlyDictionary<string, int> GetAllStock();

    /// <summary>
    /// Attempts to sell the given quantity of a SKU through a channel.
    /// Returns false (with no partial deduction) if insufficient stock or unknown SKU.
    /// </summary>
    bool TrySell(string sku, int quantity, Channel channel);

    /// <summary>Recent sale events, most recent first.</summary>
    IReadOnlyList<SaleEvent> RecentSales { get; }

    /// <summary>Restores seed catalog stock levels and clears simulated sales history.</summary>
    void Reset();
}
