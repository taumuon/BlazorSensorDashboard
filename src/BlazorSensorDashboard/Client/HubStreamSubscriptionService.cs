using BlazorSensorDashboard.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;

namespace BlazorSensorDashboard.Client
{
    public class HubStreamSubscriptionService
    {
        HubConnectionService hubConnectionService;

        Dictionary<string, IObservable<SensorReading>> sensorObservables = new Dictionary<string, IObservable<SensorReading>>();

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
            // TODO: abstract out with sensor manager
            lock (_syncLock)
            {
                if (sensorObservables.TryGetValue(sensorName, out var obs))
                {
                    Console.WriteLine("Returning existing connection for " + streamName + ' ' + sensorName);
                    return obs;
                }

                Console.WriteLine("subscribing to " + streamName + ' ' + sensorName);

                var sensorObservable = Observable.Create<SensorReading>(async observer =>
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    Console.WriteLine($"Subscribing to channel async for streamName: {streamName} sensorName: {sensorName}");
                    hubConnection.StreamAsync<SensorReading>(streamName, sensorName, cancellationTokenSource.Token);
                    var ch = await hubConnection.StreamAsChannelAsync<SensorReading>(streamName, sensorName, cancellationTokenSource.Token);
                    while (await ch.WaitToReadAsync())
                    {
                        while (ch.TryRead(out var sensorReading))
                        {
                            observer.OnNext(sensorReading);
                        }
                    }

                    Console.WriteLine("Completed");
                    observer.OnCompleted();

                    return cancellationTokenSource;
                });

                var obsNew = sensorObservable
                            .Finally(() =>
                            {
                                lock (_syncLock)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Removing last subscription for streamName: {streamName} sensorName: {sensorName}");
                                    sensorObservables.Remove(sensorName);
                                }
                            })
                            .Publish()
                            .RefCount();

                sensorObservables[sensorName] = obsNew;

                return obsNew;
            }
        }
        private readonly object _syncLock = new object();
    }
}
