using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IIndexedEnumerable<T> : IEnumerable<T>
    {
        T this[int index] { get; }

        int Count { get; }
    }
}
