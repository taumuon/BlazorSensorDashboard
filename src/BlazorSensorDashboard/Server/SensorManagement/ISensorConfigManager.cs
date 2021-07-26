using BlazorSensorDashboard.Shared;
using System.Collections.Generic;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public interface ISensorConfigManager
    {
        IEnumerable<SensorConfigInfo> Sensors { get; }

        bool TryAdd(SensorConfigInfo sensor);
    }
}
