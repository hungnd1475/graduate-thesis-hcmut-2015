using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace HCMUT.EMRCorefResol.Utilities
{
    class Cache<TKey, TValue>
    {
        private readonly LinkedList<TKey> _keyList;
        private readonly Dictionary<TKey, TValue> _valueMap;
    }
}
