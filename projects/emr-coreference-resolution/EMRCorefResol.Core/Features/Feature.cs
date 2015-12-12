using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Default IFeature implementation for convenience.
    /// </summary>
    public abstract class Feature : IFeature
    {
        public string Name { get; }

        public double[] Value { get; }

        public bool IsCategorical { get; }

        /// <summary>
        /// Initialize a categorical <see cref="Feature"/> instane with specified initial value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="initValue"></param>
        public Feature(string name, int size, int initValue)
            : this(name, new double[size], true)
        {
            SetCategoricalValue(initValue);
        }

        /// <summary>
        /// Initialize a categorical <see cref="Feature"/> instane with initial value set to 0.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        public Feature(string name, int size)
            : this(name, new double[size], true)
        {
            SetCategoricalValue(0);
        }

        /// <summary>
        /// Initialize a continuous <see cref="Feature"/> instance with specified initial value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="initValue"></param>
        public Feature(string name, double initValue)
            : this(name, new double[1], false)
        {
            SetContinuousValue(initValue);
        }

        /// <summary>
        /// Initialize a continuous <see cref="Feature"/> instance with initial value set to 0.
        /// </summary>
        /// <param name="name"></param>
        public Feature(string name)
            : this(name, new double[1], false)
        {
            SetContinuousValue(0d);
        }

        private Feature(string name, double[] initValue, bool isCategorical)
        {
            if (initValue.Length <= 0)
            {
                throw new ArgumentException("Value size must be greater than 0.");
            }
            else if (isCategorical && initValue.Length <= 1)
            {
                throw new ArgumentException("Value size for categorical feature must be at least 2.");
            }

            Name = name;
            Value = initValue;
            IsCategorical = isCategorical;
        }

        protected void SetCategoricalValue(int value)
        {
            if (value >= 0 || value < Value.Length)
            {
                for (int i = 0; i < Value.Length; i++)
                {
                    Value[i] = i == value ? 1d : 0d;
                }
            }
        }

        protected void SetContinuousValue(double value)
        {
            Value[0] = value;
        }

        public int GetCategoricalValue()
        {
            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i] != 0)
                    return i;
            }
            return 0;
        }

        public double GetContinuousValue()
        {
            return Value[0];
        }

        public override string ToString()
        {
            var value = IsCategorical ? GetCategoricalValue() : GetContinuousValue();
            return $"{Name}:{value}";
        }

        public override int GetHashCode()
        {
            var values = new List<object>();
            values.Add(Name);
            foreach (var v in Value)
                values.Add(v);

            return HashCodeHelper.ComputeHashCode(values.ToArray());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IFeature);
        }

        public bool Equals(IFeature other)
        {
            if (other == null)
                return false;

            if (Name != other.Name)
                return false;

            if (Value.Length != other.Value.Length)
                return false;
            
            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i] != other.Value[i])
                    return false;
            }

            return true;
        }
    }
}
