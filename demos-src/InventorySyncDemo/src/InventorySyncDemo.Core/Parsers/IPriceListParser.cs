using InventorySyncDemo.Core.Models;

namespace InventorySyncDemo.Core.Parsers;

/// <summary>
/// Parses a supplier's raw price-list file content into normalized entries. Each supplier
/// sends a different column layout / delimiter / header naming - a distinct implementation
/// per supplier is what proves "we normalize whatever format your suppliers send".
/// </summary>
public interface IPriceListParser
{
    /// <summary>The fictional supplier name this parser understands.</summary>
    string SupplierName { get; }

    /// <summary>A short description of this supplier's raw file format, for display in the UI.</summary>
    string FormatDescription { get; }

    IReadOnlyList<SupplierPriceListEntry> Parse(string fileContent);
}
