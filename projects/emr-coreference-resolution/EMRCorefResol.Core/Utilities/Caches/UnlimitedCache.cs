using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace HCMUT.EMRCorefResol.Utilities
{
    /// <summary>
    /// An <see cref="ICache{TKey, TValue}"/> implementation that stores values as long as there is memory available.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UnlimitedCache<TKey, TValue> : ICache<TKey, TValue>
    {
        //private readonly object _syncObj = new object();
        private readonly ConcurrentDictionary<TKey, TValue> _values
            = new ConcurrentDictionary<TKey, TValue>();

        public void Clear()
        {
            _values.Clear();
        }

        public TValue GetValue(TKey key, Func<TKey, TValue> valueProducer)
        {
            return _values.GetOrAdd(key, valueProducer);
        }
    }
}
