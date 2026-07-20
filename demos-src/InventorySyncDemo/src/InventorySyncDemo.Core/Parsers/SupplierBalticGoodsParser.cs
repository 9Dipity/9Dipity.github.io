using System.Globalization;
using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Parsers;

/// <summary>
/// "Baltic Goods" sends a semicolon-delimited file with different header names and
/// European-style decimal-comma prices. Format: Article Code;Item Description;Unit Price
/// e.g. "NFH-SWT-002;Cable Knit Cardigan;92,50"
/// </summary>
public sealed class SupplierBalticGoodsParser : IPriceListParser
{
    public string SupplierName => "Baltic Goods";
    public string FormatDescription => "Semicolon-delimited CSV, decimal-comma prices - Article Code;Item Description;Unit Price";

    public IReadOnlyList<SupplierPriceListEntry> Parse(string fileContent)
    {
        var results = new List<SupplierPriceListEntry>();
        var lines = SupplierNordicTextilesParser.SplitLines(fileContent);

        foreach (var line in lines.Skip(1)) // skip header row
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Split(';');
            if (columns.Length < 3) continue;

            var sku = columns[0].Trim();
            // European format uses comma as the decimal separator, e.g. "92,50"
            var priceRaw = columns[2].Trim().Replace(",", ".");

            if (!decimal.TryParse(priceRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var newPrice))
                continue;

            results.Add(new SupplierPriceListEntry(sku, SupplierName, newPrice));
        }

        return results;
    }
}
