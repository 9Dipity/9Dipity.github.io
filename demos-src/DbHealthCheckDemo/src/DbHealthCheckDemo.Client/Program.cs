using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DbHealthCheckDemo.Client;
using DbHealthCheckDemo.Core.Localization;
using DbHealthCheckDemo.Core.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Everything below is in-memory only - there is no server and no HTTP calls for app
// data. IDiagnosticEngine is a pure function over a DatabaseProfile, so it's registered
// as a singleton purely for consistency with the other demos' service pattern.
builder.Services.AddSingleton<IDiagnosticEngine, DiagnosticEngine>();
builder.Services.AddSingleton<ILocalizer, DictionaryLocalizer>();

await builder.Build().RunAsync();
