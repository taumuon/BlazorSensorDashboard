using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorSensorDashboard.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                var loggerProvider = sp.GetRequiredService<ILoggerProvider>();
                var hubConnectionService = new HubConnectionService(navigationManager, loggerProvider);
                return hubConnectionService;
            });

            builder.Services.AddSingleton(sp =>
            {
                var hubConnectionService = sp.GetRequiredService<HubConnectionService>();
                var hubSubscriptionService = new HubStreamSubscriptionService(hubConnectionService);
                return hubSubscriptionService;
            });

            builder.Services.AddSingleton(sp =>
            {
                var hubConnectionService = sp.GetRequiredService<HubConnectionService>();
                var jsRuntime = sp.GetRequiredService<IJSRuntime>();
                var configurationService = new ConfigurationService(hubConnectionService, jsRuntime);
                return configurationService;
            });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
