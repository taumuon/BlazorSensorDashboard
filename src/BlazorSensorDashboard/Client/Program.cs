using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                var hubSubscriptionService = new HubStreamSubscriptionService(hubConnectionService);
                return hubSubscriptionService;
            });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
