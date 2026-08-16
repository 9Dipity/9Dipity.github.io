using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServiceBoardDemo.Client;
using ServiceBoardDemo.Core.Localization;
using ServiceBoardDemo.Core.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Everything below is in-memory only - there is no server and no HTTP calls for app
// data. IJobBoardService is a singleton so the board page and the parts page share the
// exact same job state.
builder.Services.AddSingleton<IJobBoardService, JobBoardService>();
builder.Services.AddSingleton<ILocalizer, DictionaryLocalizer>();

await builder.Build().RunAsync();
