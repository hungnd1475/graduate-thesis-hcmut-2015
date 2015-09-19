using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp;

namespace HCMUT.EMRCorefResol.English.SVM
{
    interface ISVMParamOptimizer
    {
        SVMParameter Optimize(SVMProblem problem);
    }
}
