using BlazorSensorDashboard.Server.SensorManagement;
using BlazorSensorDashboard.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BlazorSensorDashboard.Server.Hubs
{
    public class StreamHub : Hub
    {
        private ISensorManager _sensorManager;

        public StreamHub(ISensorManager sensorManager)
        {
            _sensorManager = sensorManager;
        }

        [HubMethodName("StartListening")]
        public ChannelReader<SensorReading> StartListening(string sensorIdentifier, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine($"Start listening for sensor {sensorIdentifier}");
            Console.WriteLine($"Start listening for sensor {sensorIdentifier}");
            var channel = Channel.CreateUnbounded<SensorReading>();

            _sensorManager.GetSensorObservable(sensorIdentifier)
                .Subscribe(val =>
                {
                    var sensorReading = new SensorReading
                    {
                        Value = val,
                        Timestamp = DateTime.Now
                    };
                    _ = Write(channel.Writer, sensorReading, cancellationToken);
                }, cancellationToken);


            return channel.Reader;
        }

        private async Task Write(ChannelWriter<SensorReading> writer, SensorReading val, CancellationToken cancellationToken)
        {
            try
            {
                await writer.WriteAsync(val, cancellationToken);
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
            }
        }
    }
}
