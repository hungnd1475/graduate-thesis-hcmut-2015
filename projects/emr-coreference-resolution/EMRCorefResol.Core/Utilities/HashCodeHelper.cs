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
            return ComputeHashCode(37, 41, values);
        }

        public static int ComputeHashCode(int prime1, int prime2, params object[] values)
        {
            unchecked
            {
                int hash = prime1;
                foreach (var v in values)
                {
                    hash = hash * prime2 + v.GetHashCode();
                }
                return hash;
            }
        }
    }
}
