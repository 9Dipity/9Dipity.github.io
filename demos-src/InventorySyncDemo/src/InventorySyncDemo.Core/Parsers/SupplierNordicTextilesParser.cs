using System.Globalization;
using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Parsers;

/// <summary>
/// "Nordic Textiles" sends a clean, comma-delimited CSV with an English header row and
/// plain decimal-point prices. Format: Sku,ProductName,PriceEUR
/// </summary>
public sealed class SupplierNordicTextilesParser : IPriceListParser
{
    public string SupplierName => "Nordic Textiles";
    public string FormatDescription => "Comma-delimited CSV - Sku,ProductName,PriceEUR";

    public IReadOnlyList<SupplierPriceListEntry> Parse(string fileContent)
    {
        var results = new List<SupplierPriceListEntry>();
        var lines = SplitLines(fileContent);

        foreach (var line in lines.Skip(1)) // skip header row
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Split(',');
            if (columns.Length < 3) continue;

            var sku = columns[0].Trim();
            var price = columns[2].Trim();

            if (!decimal.TryParse(price, NumberStyles.Number, CultureInfo.InvariantCulture, out var newPrice))
                continue;

            results.Add(new SupplierPriceListEntry(sku, SupplierName, newPrice));
        }

        return results;
    }

    internal static string[] SplitLines(string content) =>
        content.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
}
