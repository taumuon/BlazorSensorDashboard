using BlazorSensorDashboard.Shared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public class SensorConfigManager : ISensorConfigManager
    {
        private ConcurrentDictionary<string, SensorConfigInfo> _sensors = new ConcurrentDictionary<string, SensorConfigInfo>(
            new Dictionary<string, SensorConfigInfo>
        {
                    {"temperature_room_1", new SensorConfigInfo { Name = "temperature_room_1", Manufacturer = "Acme", HostDevice = "RaspberryPiRoom1", SensorUnits = "℃"} },
                    {"temperature_room_2", new SensorConfigInfo { Name = "temperature_room_2", Manufacturer = "Acme", HostDevice = "RaspberryPiRoom2", SensorUnits = "℃"} },
                    {"humidity_room_2", new SensorConfigInfo { Name = "humidity_room_2", Manufacturer = "Acme", HostDevice = "RaspberyPiRoom1", SensorUnits = "%" } }
        });

        public IEnumerable<SensorConfigInfo> Sensors => _sensors.Values.ToArray();

        public bool TryAdd(SensorConfigInfo sensor)
        {
            return _sensors.TryAdd(sensor.Name, sensor);
        }
    }
}
