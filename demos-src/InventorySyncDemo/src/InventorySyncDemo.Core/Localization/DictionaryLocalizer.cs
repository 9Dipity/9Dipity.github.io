namespace InventorySyncDemo.Core.Localization;

/// <summary>
/// Simple in-memory dictionary translation service. Default language is Latvian (the
/// primary audience for this demo is Riga-area retailers/distributors); English is
/// available via the language toggle. Switching language only changes CurrentLanguage
/// and fires LanguageChanged - it never touches stock/sales state.
/// </summary>
public sealed class DictionaryLocalizer : ILocalizer
{
    public Language CurrentLanguage { get; private set; } = Language.Lv;

    public event Action? LanguageChanged;

    public void SetLanguage(Language language)
    {
        if (CurrentLanguage == language) return;
        CurrentLanguage = language;
        LanguageChanged?.Invoke();
    }

    public string T(string key)
    {
        if (Translations.TryGetValue(key, out var byLanguage) &&
            byLanguage.TryGetValue(CurrentLanguage, out var value))
        {
            return value;
        }

        return key; // fallback so a missing key is visible rather than throwing
    }

    private static readonly Dictionary<string, Dictionary<Language, string>> Translations = new()
    {
        ["nav.brand"] = New("Nordic Fashion House", "Nordic Fashion House"),
        ["nav.brandSubtitle"] = New("Ops Console", "Vadības panelis"),
        ["nav.dashboard"] = New("Dashboard", "Panelis"),
        ["nav.priceLists"] = New("Price Lists", "Cenrāži"),

        ["dashboard.title"] = New("Inventory Command Center", "Krājumu vadības centrs"),
        ["dashboard.subtitle"] = New(
            "One shared stock pool across every channel — sell anywhere, see it everywhere, instantly.",
            "Viens kopīgs krājumu fonds visiem kanāliem — pārdod jebkur, redzi izmaiņas visur, uzreiz."),
        ["dashboard.resetButton"] = New("Reset Demo Data", "Atiestatīt demo datus"),
        ["dashboard.catalogHeading"] = New("Shared Product Catalog", "Kopīgais preču katalogs"),
        ["dashboard.lowStock"] = New("Low stock", "Zems krājums"),
        ["dashboard.outOfStock"] = New("Out of stock", "Nav pieejams"),
        ["dashboard.column.product"] = New("Product", "Prece"),
        ["dashboard.column.category"] = New("Category", "Kategorija"),
        ["dashboard.column.price"] = New("Price", "Cena"),
        ["dashboard.column.stock"] = New("Stock", "Krājums"),

        ["channel.RetailStoreA"] = New("Retail Store A", "Veikals A"),
        ["channel.RetailStoreB"] = New("Retail Store B", "Veikals B"),
        ["channel.LivestreamOnline"] = New("Livestream / Online Orders", "Tiešraides pasūtījumi"),

        ["channel.simulateOrder"] = New("Simulate order", "Simulēt pasūtījumu"),
        ["channel.pickProduct"] = New("Pick a product…", "Izvēlies preci…"),
        ["channel.randomProduct"] = New("Random in-stock item", "Nejauša prece"),
        ["channel.recentActivity"] = New("Recent activity", "Nesenā aktivitāte"),
        ["channel.noActivity"] = New("No sales yet — try simulating an order.", "Vēl nav pārdošanas — mēģini simulēt pasūtījumu."),
        ["channel.insufficientStock"] = New("Not enough stock for that item.", "Šai precei nepietiek krājuma."),
        ["channel.soldOne"] = New("Sold 1 × {0} — updated everywhere instantly.", "Pārdota 1 × {0} — atjaunināts visur uzreiz."),

        ["priceLists.title"] = New("Supplier Price List Import", "Piegādātāju cenrāžu imports"),
        ["priceLists.subtitle"] = New(
            "Upload any supplier's price list — we normalize it automatically, whatever format it arrives in.",
            "Augšupielādē jebkura piegādātāja cenrādi — mēs to automātiski normalizējam, lai kāds arī būtu formāts."),
        ["priceLists.chooseSupplier"] = New("Choose supplier format", "Izvēlies piegādātāja formātu"),
        ["priceLists.uploadLabel"] = New("Upload CSV file", "Augšupielādēt CSV failu"),
        ["priceLists.downloadSample"] = New("Download sample CSV", "Lejupielādēt parauga CSV"),
        ["priceLists.formatLabel"] = New("Format", "Formāts"),
        ["priceLists.resultsHeading"] = New("Normalized Price Changes", "Normalizētās cenu izmaiņas"),
        ["priceLists.column.product"] = New("Product", "Prece"),
        ["priceLists.column.supplier"] = New("Supplier", "Piegādātājs"),
        ["priceLists.column.previousPrice"] = New("Previous Price", "Iepriekšējā cena"),
        ["priceLists.column.newPrice"] = New("New Price", "Jaunā cena"),
        ["priceLists.column.change"] = New("Change", "Izmaiņa"),
        ["priceLists.direction.up"] = New("Up", "Augšup"),
        ["priceLists.direction.down"] = New("Down", "Lejup"),
        ["priceLists.direction.same"] = New("No change", "Bez izmaiņām"),
        ["priceLists.noFile"] = New("Upload a CSV to see normalized results here.", "Augšupielādē CSV, lai šeit redzētu normalizētos rezultātus."),
        ["priceLists.parseError"] = New(
            "Could not parse this file with the selected supplier format. Double-check you picked the right supplier.",
            "Neizdevās nolasīt šo failu ar izvēlēto piegādātāja formātu. Pārbaudi, vai izvēlēts pareizais piegādātājs."),
        ["priceLists.rowsParsed"] = New("rows parsed", "rindas nolasītas"),

        ["footer.disclaimer"] = New(
            "This is a demo built by DataCraft Consulting to illustrate what's possible — not a real client's system or data.",
            "Šī ir DataCraft Consulting izveidota demonstrācijas versija, kas parāda iespējas — nevis reāla klienta sistēma vai dati."),

        ["lang.en"] = New("EN", "EN"),
        ["lang.lv"] = New("LV", "LV"),
    };

    private static Dictionary<Language, string> New(string en, string lv) => new()
    {
        [Language.En] = en,
        [Language.Lv] = lv
    };
}
