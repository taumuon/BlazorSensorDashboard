using System;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public interface ISensorManager
    {
        IObservable<double> GetSensorObservable(string sensorIdentifier);
    }
}
