using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public interface IPerfMetric
    {
        string Name { get; }
        Dictionary<ConceptType, Evaluation> Evaluate(EMR emr, CorefChainCollection groundTruth,
            CorefChainCollection systemChains);
    }
}
