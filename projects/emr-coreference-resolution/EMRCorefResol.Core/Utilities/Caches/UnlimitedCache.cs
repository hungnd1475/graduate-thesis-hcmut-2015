using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    /// <summary>
    /// An <see cref="ICache{TKey, TValue}"/> implementation that stores values as long as there is memory available.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UnlimitedCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly object _syncObj = new object();
        private readonly Dictionary<TKey, TValue> _values
            = new Dictionary<TKey, TValue>();

        public void Clear()
        {
            _values.Clear();
        }

        public TValue GetValue(TKey key, Func<TKey, TValue> valueProducer)
        {
            lock (_syncObj)
            {
                if (!_values.ContainsKey(key))
                {
                    _values.Add(key, valueProducer(key));
                }
            }
            return _values[key];
        }
    }
}
