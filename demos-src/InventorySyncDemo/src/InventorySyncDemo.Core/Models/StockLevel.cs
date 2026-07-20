namespace InventorySyncDemo.Core.Models;

/// <summary>
/// Current on-hand quantity for a SKU. There is exactly ONE stock level per SKU shared
/// across every channel - not a per-channel copy. That is the whole point of the demo.
/// </summary>
public sealed class StockLevel
{
    public string Sku { get; }
    public int Quantity { get; internal set; }

    public StockLevel(string sku, int quantity)
    {
        Sku = sku;
        Quantity = quantity;
    }
}
