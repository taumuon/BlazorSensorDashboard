using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public class SensorManager : ISensorManager
    {
        private readonly Dictionary<string, ISensor> _sensors = new Dictionary<string, ISensor>
            {
                { "temperature_room_1", new FakeSensor(25) },
                { "temperature_room_2", new FakeSensor(20) },
                { "humidity_room_2", new FakeSensor(45) }
            };

        private readonly Dictionary<string, IObservable<double>> _sensorObservables = new Dictionary<string, IObservable<double>>();
        private readonly object _syncLock = new object();


        public IObservable<double> GetSensorObservable(string sensorIdentifier)
        {
            lock (_syncLock)
            {
                IObservable<double> obs;
                if (!_sensorObservables.TryGetValue(sensorIdentifier, out obs))
                {
                    System.Diagnostics.Debug.WriteLine($"SensorManager adding {sensorIdentifier}");
                    var source = _sensors[sensorIdentifier].GetReadings();

                    obs = source
                        .Finally(() =>
                        {
                            lock (_syncLock)
                            {
                                System.Diagnostics.Debug.WriteLine($"SensorManager removing {sensorIdentifier}");
                                _sensorObservables.Remove(sensorIdentifier);
                            }
                        })
                        .Publish()
                        .RefCount();

                    _sensorObservables.Add(sensorIdentifier, obs);
                }
                return obs;
            }
        }
    }
}
