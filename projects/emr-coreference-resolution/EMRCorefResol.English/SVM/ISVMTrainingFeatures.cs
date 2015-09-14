using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.SVM
{
    interface ISVMTrainingFeatures : IFeatureVector
    {
        void AddTo(SVMProblems problems);
    }
}
