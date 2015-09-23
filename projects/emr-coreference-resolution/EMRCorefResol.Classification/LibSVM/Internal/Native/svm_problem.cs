using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification.LibSVM.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    public struct svm_problem
    {
        public int l;
        public IntPtr y; // double*
        public IntPtr x; // svm_node**
    }
}
