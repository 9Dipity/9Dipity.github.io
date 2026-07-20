namespace InventorySyncDemo.Core.Parsers;

/// <summary>
/// Maps a chosen supplier to the parser implementation that understands its file format.
/// Adding a new supplier is just registering one more IPriceListParser here.
/// </summary>
public interface IPriceListParserResolver
{
    IReadOnlyList<IPriceListParser> AvailableParsers { get; }

    IPriceListParser Resolve(string supplierName);
}
