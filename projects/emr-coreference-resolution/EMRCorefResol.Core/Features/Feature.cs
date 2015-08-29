using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public abstract class Feature : IFeature
    {
        public string Name { get; }

        public double Value { get; protected set; }

        public Feature(string name)
        {
            Name = name;
        }
    }
}
