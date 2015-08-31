using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        {
            _features = new IFeature[size];
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return _features.AsEnumerable().GetEnumerator();
        }

        public double[] ToDoubleArray()
        {
            return _features.Select(f => f.Value).ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
