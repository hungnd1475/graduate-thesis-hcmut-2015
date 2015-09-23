using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public class ClasProblem
    {
        public List<IFeatureVector> X { get; } = new List<IFeatureVector>();

        public List<double> Y { get; } = new List<double>();

        public int Size { get { return X.Count; } }

        public void Add(IFeatureVector fVector, double classValue)
        {
            X.Add(fVector);
            Y.Add(classValue);
        }
    }
}
