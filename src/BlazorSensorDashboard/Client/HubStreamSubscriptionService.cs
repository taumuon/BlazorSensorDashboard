using BlazorSensorDashboard.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Reactive.Linq;
using System.Threading;

namespace BlazorSensorDashboard.Client
{
    public class HubStreamSubscriptionService
    {
        HubConnectionService hubConnectionService;

        SharedObservables<string, SensorReading> sharedObservables = new SharedObservables<string, SensorReading>();

        public HubStreamSubscriptionService(HubConnectionService hubConnectionService)
        {
            this.hubConnectionService = hubConnectionService;
        }

        public IObservable<SensorReading> SubscribeStream(string streamName, string sensorName)
        {
            Console.WriteLine($"Getting connection to /streamHub for streamname {streamName} sensor {sensorName}");

            return this.hubConnectionService.GetConnection("/streamHub")
                .SelectMany(hubConnection =>
                {
                    Console.WriteLine($"Got connection to /streamHub streamName {streamName} for sensor {sensorName}");
                    return SubscribeStreamToHub(streamName, sensorName, hubConnection);
                });
        }

        public IObservable<SensorReading> SubscribeStreamToHub(string streamName, string sensorName, HubConnection hubConnection)
        {
            return sharedObservables.GetObservable(sensorName, () =>
            {
                var sensorObservable = Observable.Create<SensorReading>(async observer =>
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    Console.WriteLine($"Subscribing to channel async for streamName: {streamName} sensorName: {sensorName}");
                    hubConnection.StreamAsync<SensorReading>(streamName, sensorName, cancellationTokenSource.Token);
                    await foreach (var sensorReading in hubConnection.StreamAsync<SensorReading>(streamName, sensorName, cancellationTokenSource.Token))
                    {
                        observer.OnNext(sensorReading);
                    }

                    Console.WriteLine("Completed");
                    observer.OnCompleted();

                    return cancellationTokenSource;
                });
                return sensorObservable;
            });
        }
    }
}
