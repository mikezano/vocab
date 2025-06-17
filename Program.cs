using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vocab;
using Vocab.Api;
using Vocab.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddScoped(sp => new GoogleSheet(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }));
//builder.Services.AddScoped(sp => new AppState());

builder.Services.AddSingleton<GoogleSheet>();
builder.Services.AddSingleton<AppState>();

await builder.Build().RunAsync();
