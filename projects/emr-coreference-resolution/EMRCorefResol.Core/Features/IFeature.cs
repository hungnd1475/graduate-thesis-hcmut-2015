using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IFeature
    {
        string Name { get; }
        double[] Value { get; }
        bool IsCategorical { get; }

        int GetCategoricalValue();
        double GetContinuousValue();
    }
}
