using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using InventorySyncDemo.Client;
using InventorySyncDemo.Core.Localization;
using InventorySyncDemo.Core.Parsers;
using InventorySyncDemo.Core.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Everything below is in-memory only - there is no server and no HTTP calls for app
// data. IStockService is a singleton so every component (every channel panel, the
// catalog table, the price-list page) shares the exact same stock state.
builder.Services.AddSingleton<IStockService, StockService>();
builder.Services.AddSingleton<IPriceListParserResolver, PriceListParserResolver>();
builder.Services.AddSingleton<IPriceNormalizationService, PriceNormalizationService>();
builder.Services.AddSingleton<ILocalizer, DictionaryLocalizer>();

await builder.Build().RunAsync();
