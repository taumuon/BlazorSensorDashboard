using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BlazorSensorDashboard.Server.SensorManagement
{
    public class FakeSensor : ISensor
    {
        private double _initialValue;

        public IObservable<double> GetReadings()
        {
            return Observable.Create<double>(async (obs, token) =>
            {
                var currentValue = _initialValue;
                var random = new Random();
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(500);
                    currentValue += (random.NextDouble() - 0.5);
                    obs.OnNext(currentValue);
                }

                return Disposable.Create(() => System.Diagnostics.Debug.WriteLine("Fake sensor disconnected"));
            });
        }

        public FakeSensor(float initialValue)
        {
            _initialValue = initialValue;
        }
    }
}
