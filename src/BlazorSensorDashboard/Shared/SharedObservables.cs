using System;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace BlazorSensorDashboard.Shared
{
    public class SharedObservables<TKey, TObs>
    {
        private Dictionary<TKey, IObservable<TObs>> _observables = new Dictionary<TKey, IObservable<TObs>>();

        private readonly object _syncLock = new object();

        public IObservable<TObs> GetObservable(TKey key, Func<IObservable<TObs>> getObservable)
        {
            lock (_syncLock)
            {
                IObservable<TObs> obs;
                if (!_observables.TryGetValue(key, out obs))
                {
                    System.Diagnostics.Debug.WriteLine($"Creating and adding obs for {key}");
                    var source = getObservable();

                    obs = source
                        .Finally(() =>
                        {
                            lock (_syncLock)
                            {
                                System.Diagnostics.Debug.WriteLine($"SensorManager removing {key}");
                                _observables.Remove(key);
                            }
                        })
                        .Publish()
                        .RefCount();

                    _observables.Add(key, obs);
                }
                return obs;
            }
        }
    }
}
