namespace InventorySyncDemo.Core.Parsers;

public sealed class PriceListParserResolver : IPriceListParserResolver
{
    public IReadOnlyList<IPriceListParser> AvailableParsers { get; }

    public PriceListParserResolver()
        : this(new IPriceListParser[]
        {
            new SupplierNordicTextilesParser(),
            new SupplierBalticGoodsParser(),
            new SupplierScandiHomeParser()
        })
    {
    }

    public PriceListParserResolver(IEnumerable<IPriceListParser> parsers)
    {
        AvailableParsers = parsers.ToList();
    }

    public IPriceListParser Resolve(string supplierName)
    {
        var match = AvailableParsers.FirstOrDefault(p =>
            string.Equals(p.SupplierName, supplierName, StringComparison.OrdinalIgnoreCase));

        return match ?? throw new ArgumentException($"No parser registered for supplier '{supplierName}'.", nameof(supplierName));
    }
}
