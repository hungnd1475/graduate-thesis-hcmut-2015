using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Provides a base class for easy implementing <see cref="IFeatureVector"/>.
    /// </summary>
    public abstract class FeatureVector : IFeatureVector
    {
        private readonly IFeature[] _features;

        public IFeature this[int index]
        {
            get { return _features[index]; }
            protected set { _features[index] = value; }
        }

        public double ClassValue { get; set; }

        public int Size
        {
            get { return _features.Length; }
        }

        public FeatureVector(int size)
            : this(size, 0.0)
        { }

        public FeatureVector(int size, double classValue)
        {
            _features = new IFeature[size];
            ClassValue = classValue;
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return _features.AsEnumerable().GetEnumerator();
        }

        public double[] ToDoubleArray()
        {
            var result = new List<double>();
            foreach (var f in _features)
            {
                for (int i = 0; i < f.Value.Length; i++)
                {
                    result.Add(f.Value[i]);
                }
            }
            return result.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
