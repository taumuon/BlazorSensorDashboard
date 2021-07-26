using System;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public interface ISensor
    {
        IObservable<double> GetReadings();
    }
}
