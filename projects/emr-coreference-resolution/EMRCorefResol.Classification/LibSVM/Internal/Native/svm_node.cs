using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification.LibSVM.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    struct svm_node
    {
        public int index;
        public double value;
    }
}
