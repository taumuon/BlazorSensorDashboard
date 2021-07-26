using BlazorSensorDashboard.Shared;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public class SensorConfigUpdate
    {
        public bool IsAdd { get; set; }

        public SensorConfigInfo SensorConfig { get; set; }
    }
}
