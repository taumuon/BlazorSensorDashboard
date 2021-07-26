namespace BlazorSensorDashboard.Client
{
    public enum DashboardItemType
    {
        Value,
        Chart
    }

    public class DashboardConfigItem
    {
        public string SensorName { get; set; }
        public DashboardItemType Type { get; set; }
        public bool MinMax { get; set; }
    }
}
