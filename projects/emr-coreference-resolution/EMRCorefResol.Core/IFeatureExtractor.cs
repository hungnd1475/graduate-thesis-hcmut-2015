using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IFeatureExtractor
    {
        EMR EMR { get; }
        IFeatureVector Extract(PersonPair instance);
        IFeatureVector Extract(ProblemPair instance);
        IFeatureVector Extract(TreatmentPair instance);
        IFeatureVector Extract(TestPair instance);
        IFeatureVector Extract(PronounInstance instance);
        IFeatureVector Extract(PersonInstance instance);
    }
}
