using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Provides a base class for easy implementing <see cref="IFeatureVector"/> interface
    /// using <see cref="Dictionary{TKey, TValue}"/> as internal implementation.
    /// </summary>
    public class FeatureVector : IFeatureVector
    {
        private readonly Dictionary<string, IFeature> _features = new Dictionary<string, IFeature>();
        private readonly List<string> _names = new List<string>();

        public IFeature this[int index]
        {
            get { return _features[_names[index]]; }
        }

        public IFeature this[string name]
        {
            get { return _features[name]; }
        }

        public double ClassValue { get; set; }

        public int Count
        {
            get { return _features.Count; }
        }

        public IIndexedEnumerable<string> FeatureNames
        {
            get { return _names.ToIndexedEnumerable(); }
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return _features.Values.GetEnumerator();
        }

        public double[] ToDoubleArray()
        {
            return _features.Values.Select(f => f.Value).ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void AddFeature(IFeature f)
        {
            _features.Add(f.Name, f);
            _names.Add(f.Name);
        }
    }
}
