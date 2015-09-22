using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public interface ITrainer 
    {
        void Train<T>(ClasProblem problem) where T : IClasInstance;
        void Train(Type instanceType, ClasProblem problem);

        IClassifier GetClassifier();
        IClasProblemSerializer ProblemSerializer { get; }
    }
}
