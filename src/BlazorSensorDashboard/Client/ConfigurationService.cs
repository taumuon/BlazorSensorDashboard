using BlazorSensorDashboard.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Microsoft.JSInterop;

namespace BlazorSensorDashboard.Client
{
    public class ConfigurationService
    {
        Subject<SensorConfigInfo[]> SensorConfigSubj = new Subject<SensorConfigInfo[]>();

        HubConnection hubConnection;

        IJSRuntime jsRuntime;

        const string url = "/configurationHub";

        public ConfigurationService(HubConnectionService hubConnectionService, IJSRuntime jsRuntime)
        {
            HubConnectionService = hubConnectionService;
            this.jsRuntime = jsRuntime;

            Console.WriteLine("creating hub connection");
            StartConnection(url);
        }

        public HubConnectionService HubConnectionService { get; }

        public IObservable<SensorConfigInfo[]> GetConfig()
        {
            return SensorConfigSubj
                    .Scan((agg, curr) => agg.Concat(curr).ToArray());
        }

        public void AddSensor(SensorConfigInfo sensorConfigItem)
        {
            if (hubConnection != null)
            {
                hubConnection.InvokeAsync<string>("AddSensor", sensorConfigItem.Name, sensorConfigItem.Manufacturer, sensorConfigItem.HostDevice, sensorConfigItem.SensorUnits)
                    .ContinueWith(r =>
                    {
                        if (r.Result != "")
                        {
                            jsRuntime.InvokeVoidAsync("alert", $"unable to add sensor: {r.Result}");
                        };
                    });
            }
        }

        private void StartConnection(string url)
        {
            HubConnectionService.GetConnection(url).Subscribe(hubConnection =>
                {
                    this.hubConnection = hubConnection;
                    hubConnection.On<SensorConfigUpdate>("ConfigUpdated", item =>
                        {
                            SensorConfigSubj.OnNext(new[] { item.SensorConfig });
                        });

                    hubConnection.InvokeAsync<SensorConfigInfo[]>("Sensors")
                        .ContinueWith(data =>
                            {
                                SensorConfigSubj.OnNext(data.Result);
                            });
                });
        }
    }
}
