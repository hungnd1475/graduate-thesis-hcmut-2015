using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class Range<T> where T : IComparable<T>, IComparable
    {
        public T Min { get; }
        public T Max { get; }

        public Range(T min, T max)
        {            
            this.Min = min;
            this.Max = max;
        }
    }

    public static class Range
    {
        public static Range<T> Create<T>(T min, T max) where T : IComparable<T>, IComparable
        {
            return new Range<T>(min, max);
        }
    }
}
