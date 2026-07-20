using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClinicBookingDemo.Client;
using ClinicBookingDemo.Core.Abstractions;
using ClinicBookingDemo.Core.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Everything below is in-memory, seeded fake demo data — no HTTP calls for app data.
builder.Services.AddSingleton<IClinicDataStore, InMemoryClinicDataStore>();
builder.Services.AddSingleton<IAvailabilityService, AvailabilityService>();
builder.Services.AddSingleton<ITranslationService, TranslationService>();

await builder.Build().RunAsync();
