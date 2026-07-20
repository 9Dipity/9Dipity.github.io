namespace InventorySyncDemo.Core.Models;

public enum PriceDirection
{
    Up,
    Down,
    Same
}

/// <summary>
/// A supplier price-list row after being matched against the current catalog and
/// compared to the previous known price - this is what the UI renders as a table.
/// </summary>
public sealed record NormalizedPriceChange(
    string Sku,
    string ProductName,
    string Supplier,
    decimal PreviousPrice,
    decimal NewPrice,
    PriceDirection Direction
)
{
    public static PriceDirection ComputeDirection(decimal previousPrice, decimal newPrice)
    {
        if (newPrice > previousPrice) return PriceDirection.Up;
        if (newPrice < previousPrice) return PriceDirection.Down;
        return PriceDirection.Same;
    }
}
