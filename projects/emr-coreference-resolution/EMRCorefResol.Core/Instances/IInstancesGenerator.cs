using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IInstancesGenerator
    {
        IIndexedEnumerable<IClasInstance> Generate(EMR emr, CorefChainCollection groundTruth);
    }
}
