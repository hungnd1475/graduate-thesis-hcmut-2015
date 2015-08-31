using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IFeatureVector : IEnumerable<IFeature>
    {
        IFeature this[int index] { get; }
        int Size { get; }
        double ClassValue { get; set; }
        double[] ToDoubleArray();
    }
}
