using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Services;

/// <summary>
/// Fixed fictional catalog + starting stock for "Nordic Fashion House". Shared by the
/// stock service (for the live dashboard) and by unit tests.
/// </summary>
public static class SeedData
{
    public static IReadOnlyList<Product> Catalog { get; } = new List<Product>
    {
        new("NFH-JKT-001", "Aurora Down Jacket",        "Jackets",   189.00m, 8),
        new("NFH-JKT-002", "Fjord Waxed Parka",         "Jackets",   219.00m, 6),
        new("NFH-JKT-003", "Nordkapp Windbreaker",      "Jackets",   129.00m, 10),
        new("NFH-SWT-001", "Lambswool Crew Sweater",    "Sweaters",   79.00m, 12),
        new("NFH-SWT-002", "Cable Knit Cardigan",       "Sweaters",   89.00m, 10),
        new("NFH-SWT-003", "Merino Turtleneck",         "Sweaters",   69.00m, 12),
        new("NFH-SWT-004", "Alpine Fleece Pullover",    "Sweaters",   59.00m, 15),
        new("NFH-BOT-001", "Tundra Leather Boots",      "Boots",     159.00m,  6),
        new("NFH-BOT-002", "Birch Chelsea Boots",       "Boots",     139.00m,  6),
        new("NFH-BOT-003", "Glacier Snow Boots",        "Boots",     169.00m,  5),
        new("NFH-SHT-001", "Oslo Flannel Shirt",        "Shirts",     49.00m, 15),
        new("NFH-SHT-002", "Copenhagen Poplin Shirt",   "Shirts",     45.00m, 15),
        new("NFH-PNT-001", "Skagen Wool Trousers",      "Trousers",   99.00m, 10),
        new("NFH-PNT-002", "Helsinki Chino Pants",      "Trousers",   75.00m, 12),
        new("NFH-PNT-003", "Arctic Thermal Leggings",   "Trousers",   39.00m, 15),
        new("NFH-ACC-001", "Nordic Wool Beanie",        "Accessories", 25.00m, 20),
        new("NFH-ACC-002", "Lapland Knit Scarf",        "Accessories", 29.00m, 18),
        new("NFH-ACC-003", "Reykjavik Leather Gloves",  "Accessories", 35.00m, 15),
    };

    /// <summary>Starting stock quantities, keyed by SKU. Some SKUs start at/below their
    /// low-stock threshold so the badge is visible immediately on first load.</summary>
    public static IReadOnlyDictionary<string, int> StartingStock { get; } = new Dictionary<string, int>
    {
        ["NFH-JKT-001"] = 22,
        ["NFH-JKT-002"] = 6,   // at threshold
        ["NFH-JKT-003"] = 34,
        ["NFH-SWT-001"] = 40,
        ["NFH-SWT-002"] = 9,   // below threshold
        ["NFH-SWT-003"] = 28,
        ["NFH-SWT-004"] = 50,
        ["NFH-BOT-001"] = 3,   // below threshold
        ["NFH-BOT-002"] = 18,
        ["NFH-BOT-003"] = 5,   // at threshold
        ["NFH-SHT-001"] = 60,
        ["NFH-SHT-002"] = 12,  // below threshold
        ["NFH-PNT-001"] = 26,
        ["NFH-PNT-002"] = 33,
        ["NFH-PNT-003"] = 45,
        ["NFH-ACC-001"] = 70,
        ["NFH-ACC-002"] = 15,  // below threshold
        ["NFH-ACC-003"] = 40,
    };
}
