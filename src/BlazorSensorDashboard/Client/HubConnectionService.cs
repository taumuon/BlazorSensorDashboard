using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace BlazorSensorDashboard.Client
{
    public class HubConnectionService
    {
        Dictionary<string, ReplaySubject<HubConnection>> connectionMap = new Dictionary<string, ReplaySubject<HubConnection>>();
        private readonly object _syncLock = new object();

        public HubConnectionService(NavigationManager navigationManager, ILoggerProvider loggerProvider)
        {
            NavigationManager = navigationManager;
            LoggerProvider = loggerProvider;
        }

        public NavigationManager NavigationManager { get; }
        public ILoggerProvider LoggerProvider { get; }

        public IObservable<HubConnection> GetConnection(string url)
        {
            lock (_syncLock)
            {
                if (connectionMap.TryGetValue(url, out var conn))
                {
                    Console.WriteLine($"GetConnection for url: {url}. Connecction already exists");
                    return conn;
                }

                Console.WriteLine($"GetConnection for url: {url}. CCreating new connection");
                var newConnection = CreateConnection(url);

                connectionMap[url] = newConnection;

                return newConnection.AsObservable();
            }
        }

        private ReplaySubject<HubConnection> CreateConnection(string url)
        {
            var absoluteUrl = NavigationManager.ToAbsoluteUri(url);
            Console.WriteLine($"CreateConnection for {url} (absolute url {absoluteUrl})");

            var hubConnected = new ReplaySubject<HubConnection>(1);

            var hubConnection = new HubConnectionBuilder()
                .WithUrl(absoluteUrl)
                //.withAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddProvider(LoggerProvider);
                })
                .Build();

            hubConnection.Closed += ex => { Console.WriteLine($"hub connection closed: {ex}"); return Task.CompletedTask; };

            var t = Task.Run(async () =>
            {
                try
                {
                    await hubConnection.StartAsync().ConfigureAwait(false);
                    Console.WriteLine($"connected to {url} hubconnectionstatus: {hubConnection.State}");
                    hubConnected.OnNext(hubConnection);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error while estabilishing connection to url: " + url + " err: " + ex);
                }
                // TODO: disposing of hub connection ?
            });

            return hubConnected;
        }
    }
}
