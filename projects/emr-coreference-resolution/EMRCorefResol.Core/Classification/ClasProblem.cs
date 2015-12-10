using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public class ClasProblem
    {
        public List<double[][]> X { get; } = new List<double[][]>();

        public List<double> Y { get; } = new List<double>();

        public int Size { get { return X.Count; } }

        private HashSet<IFeatureVector> _fVectors = new HashSet<IFeatureVector>();

        public void Add(IFeatureVector fVector, double classValue)
        {
            if (!_fVectors.Contains(fVector))
            {
                X.Add(fVector.ToDoubleArray());
                Y.Add(classValue);
                _fVectors.Add(fVector);
            }
        }

        public void ClearCache()
        {
            _fVectors = null;
        }
    }
}
