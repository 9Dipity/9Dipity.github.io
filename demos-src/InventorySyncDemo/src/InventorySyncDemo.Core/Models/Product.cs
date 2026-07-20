namespace InventorySyncDemo.Core.Models;

/// <summary>
/// A sellable catalog item for Nordic Fashion House (fictional clothing retailer).
/// </summary>
public sealed record Product(
    string Sku,
    string Name,
    string Category,
    decimal Price,
    int LowStockThreshold
);
