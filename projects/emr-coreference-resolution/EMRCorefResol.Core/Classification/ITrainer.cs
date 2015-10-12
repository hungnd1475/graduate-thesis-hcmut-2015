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
        void Train<T>(ClasProblem problem, GenericConfig config) where T : IClasInstance;
        void Train(Type instanceType, ClasProblem problem, GenericConfig config);
        void Train<T>(string problemPath, GenericConfig config) where T : IClasInstance;
        void Train(Type instanceType, string problemPath, GenericConfig config);

        IClassifier GetClassifier();
        IClasProblemSerializer ProblemSerializer { get; }
    }
}
