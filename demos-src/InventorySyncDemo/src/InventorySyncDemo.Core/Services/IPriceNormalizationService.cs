using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Services;

/// <summary>
/// Matches raw supplier price-list entries against the current catalog and computes the
/// up/down/same price direction against the catalog's current price.
/// </summary>
public interface IPriceNormalizationService
{
    IReadOnlyList<NormalizedPriceChange> Normalize(IReadOnlyList<SupplierPriceListEntry> entries);
}
