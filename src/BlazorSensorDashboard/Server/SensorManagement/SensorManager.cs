using BlazorSensorDashboard.Shared;
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

        private SharedObservables<string, double> _sharedObservables = new SharedObservables<string, double>();

        public IObservable<double> GetSensorObservable(string sensorIdentifier)
        {
            return _sharedObservables.GetObservable(sensorIdentifier, () =>
            {
                return _sensors[sensorIdentifier].GetReadings();
            });
        }
    }
}
