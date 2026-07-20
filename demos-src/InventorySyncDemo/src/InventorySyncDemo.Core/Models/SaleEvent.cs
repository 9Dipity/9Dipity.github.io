namespace InventorySyncDemo.Core.Models;

/// <summary>
/// A record of a single simulated sale, used to drive the activity feed / history in the demo UI.
/// </summary>
public sealed record SaleEvent(
    string Sku,
    Channel Channel,
    int Quantity,
    DateTimeOffset Timestamp
);
