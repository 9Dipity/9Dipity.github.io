using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Services;

/// <summary>
/// In-memory implementation of the shared stock pool. Register as a singleton so every
/// component (regardless of which channel panel it renders) shares the exact same state
/// and gets notified via <see cref="StockChanged"/> when any channel sells something.
/// </summary>
public sealed class StockService : IStockService
{
    private readonly object _lock = new();
    private readonly Dictionary<string, StockLevel> _stock = new();
    private readonly List<SaleEvent> _sales = new();

    public event Action? StockChanged;

    public string? LastChangedSku { get; private set; }

    public IReadOnlyList<Product> Catalog { get; } = SeedData.Catalog;

    public IReadOnlyList<SaleEvent> RecentSales
    {
        get
        {
            lock (_lock)
            {
                return _sales.AsEnumerable().Reverse().Take(25).ToList();
            }
        }
    }

    public StockService()
    {
        SeedStock();
    }

    private void SeedStock()
    {
        lock (_lock)
        {
            _stock.Clear();
            foreach (var (sku, qty) in SeedData.StartingStock)
            {
                _stock[sku] = new StockLevel(sku, qty);
            }
        }
    }

    public int GetStock(string sku)
    {
        lock (_lock)
        {
            return _stock.TryGetValue(sku, out var level) ? level.Quantity : 0;
        }
    }

    public IReadOnlyDictionary<string, int> GetAllStock()
    {
        lock (_lock)
        {
            return _stock.ToDictionary(kv => kv.Key, kv => kv.Value.Quantity);
        }
    }

    public bool TrySell(string sku, int quantity, Channel channel)
    {
        if (quantity <= 0) return false;

        lock (_lock)
        {
            if (!_stock.TryGetValue(sku, out var level)) return false;
            if (level.Quantity < quantity) return false;

            level.Quantity -= quantity;
            _sales.Add(new SaleEvent(sku, channel, quantity, DateTimeOffset.UtcNow));
        }

        LastChangedSku = sku;
        StockChanged?.Invoke();
        return true;
    }

    public void Reset()
    {
        SeedStock();
        lock (_lock)
        {
            _sales.Clear();
        }
        LastChangedSku = null;
        StockChanged?.Invoke();
    }
}
