using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Utilities;

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

        public double[][] ToDoubleArray()
        {
            return _features.Select(f => f.Value).ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(IFeatureVector other)
        {
            if (other == null)
                return false;

            if (other.ClassValue != ClassValue)
                return false;

            if (other.Size != Size)
                return false;

            for (int i = 0; i < Size; i++)
            {
                if (!other[i].Equals(this[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IFeatureVector);
        }

        public override int GetHashCode()
        {
            var values = new List<object>();
            values.Add(ClassValue);
            foreach (var f in _features)
                values.Add(f);

            return HashCodeHelper.ComputeHashCode(values.ToArray());
        }
    }
}
