using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Services;

/// <summary>
/// Matches parsed supplier rows against the shared catalog by SKU and computes the
/// price direction against the catalog's current price.
/// </summary>
public sealed class PriceNormalizationService : IPriceNormalizationService
{
    private readonly IStockService _stockService;

    public PriceNormalizationService(IStockService stockService)
    {
        _stockService = stockService;
    }

    public IReadOnlyList<NormalizedPriceChange> Normalize(IReadOnlyList<SupplierPriceListEntry> entries)
    {
        var results = new List<NormalizedPriceChange>();
        var catalogBySku = _stockService.Catalog.ToDictionary(p => p.Sku, StringComparer.OrdinalIgnoreCase);

        foreach (var entry in entries)
        {
            if (!catalogBySku.TryGetValue(entry.SkuOrName, out var product))
                continue; // unmatched SKU - skipped

            var direction = NormalizedPriceChange.ComputeDirection(product.Price, entry.NewPrice);

            results.Add(new NormalizedPriceChange(
                product.Sku,
                product.Name,
                entry.Supplier,
                product.Price,
                entry.NewPrice,
                direction));
        }

        return results;
    }
}
