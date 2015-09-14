using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class TrainingResult
    {
        double CompleteTime { get; }
        IClassifier Classifier { get; }
    }
}
