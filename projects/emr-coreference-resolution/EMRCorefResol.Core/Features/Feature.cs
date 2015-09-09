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

        public double Value { get; protected set; }

        /// <summary>
        /// Initializes a <see cref="Feature"/> instance with specified name
        /// and value set to 0.
        /// </summary>
        /// <param name="name">The name of the feature.</param>
        public Feature(string name)
            : this(name, 0)
        { }

        /// <summary>
        /// Initializes a <see cref="Feature"/> instance with specified name and value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="initValue"></param>
        public Feature(string name, double initValue)
        {
            Name = name;
            Value = initValue;
        }

        public Feature(string name, Func<double> calculateValue)
        {
            Name = name;
            Value = calculateValue();
        }
    }
}
