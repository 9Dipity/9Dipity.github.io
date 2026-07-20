using System.Globalization;
using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Parsers;

/// <summary>
/// "Scandi Home" sends a tab-delimited file with extra columns and a different column
/// order than the other two suppliers. Rather than assuming fixed positions, this parser
/// reads the header row to locate the item code and price columns wherever they land -
/// this is the format that proves "we cope with whatever order your columns are in".
/// Typical format: Item Code\tCategory\tList Price\tCurrency
/// </summary>
public sealed class SupplierScandiHomeParser : IPriceListParser
{
    public string SupplierName => "Scandi Home";
    public string FormatDescription => "Tab-delimited file, flexible column order - Item Code / Category / List Price / Currency";

    public IReadOnlyList<SupplierPriceListEntry> Parse(string fileContent)
    {
        var results = new List<SupplierPriceListEntry>();
        var lines = SupplierNordicTextilesParser.SplitLines(fileContent);
        if (lines.Length == 0) return results;

        var headers = lines[0].Split('\t').Select(h => h.Trim()).ToArray();
        var skuIndex = FindColumn(headers, "item code", "code", "sku");
        var priceIndex = FindColumn(headers, "list price", "price");

        if (skuIndex < 0 || priceIndex < 0) return results;

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Split('\t');
            if (columns.Length <= Math.Max(skuIndex, priceIndex)) continue;

            var sku = columns[skuIndex].Trim();

            // Price may carry a trailing currency code, e.g. "169.00 EUR"
            var priceRaw = columns[priceIndex].Trim()
                .Replace("EUR", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (!decimal.TryParse(priceRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var newPrice))
                continue;

            results.Add(new SupplierPriceListEntry(sku, SupplierName, newPrice));
        }

        return results;
    }

    private static int FindColumn(string[] headers, params string[] candidates)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var header = headers[i].ToLowerInvariant();
            if (candidates.Any(c => header.Contains(c, StringComparison.OrdinalIgnoreCase)))
                return i;
        }
        return -1;
    }
}
