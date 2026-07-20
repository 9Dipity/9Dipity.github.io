namespace InventorySyncDemo.Core.Models;

/// <summary>
/// A single row parsed out of a raw supplier price-list file, before normalization
/// against the current catalog price.
/// </summary>
public sealed record SupplierPriceListEntry(
    string SkuOrName,
    string Supplier,
    decimal NewPrice
);
