using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public static class HashCodeHelper
    {
        public static int ComputeHashCode(params object[] values)
        {
            return ComputeHashCode(Tuple.Create(37, 41), values);
        }

        public static int ComputeHashCode(Tuple<int, int> primes, params object[] values)
        {
            if (values.Length <= 0)
            {
                throw new InvalidOperationException("Hash code must be computed on at least one value.");
            }

            if (values.Length == 1)
            {
                return values[0].GetHashCode();
            }
            else
            {
                unchecked
                {
                    //int hash = primes.Item1;
                    //foreach (var v in values)
                    //{
                    //    hash = hash * primes.Item2 + v.GetHashCode();
                    //}
                    return values.Aggregate(primes.Item1, (h, v) => h * primes.Item2 + v.GetHashCode());
                }
            }
        }
    }
}
