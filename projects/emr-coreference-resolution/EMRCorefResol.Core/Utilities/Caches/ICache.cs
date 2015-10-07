using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Gets value based on a specified key or produce and store the value if not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="valueProducer">The producer function called when value not found.</param>
        /// <returns></returns>
        TValue GetValue(TKey key, Func<TKey, TValue> valueProducer);

        /// <summary>
        /// Clear the cache.
        /// </summary>
        void Clear();
    }
}
