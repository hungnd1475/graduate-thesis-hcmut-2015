using HCMUT.EMRCorefResol.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface ICorefResolver
    {
        CorefChainCollection Resolve(EMR emr, IFeatureExtractor fExtractor, IClassifier classifier);
    }
}
