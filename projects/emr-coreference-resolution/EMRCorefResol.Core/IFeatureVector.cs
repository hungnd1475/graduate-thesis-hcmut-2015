using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IFeatureVector : IEnumerable<IFeature>
    {
        IFeature this[string name] { get; }
        double ClassValue { get; }
        double[] ToDoubleArray();
    }
}
