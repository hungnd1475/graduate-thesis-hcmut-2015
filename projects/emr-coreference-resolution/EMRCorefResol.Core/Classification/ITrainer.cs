using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public interface ITrainer<TClassifier> 
        where TClassifier : IClassifier
    {
        void Train<T>(ClasProblem problem) where T : IClasInstance;
        void Train(Type instanceType, ClasProblem problem);

        TClassifier GetClassifier();
        IClasProblemSerializer ProblemSerializer { get; }
    }
}
