using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class Soon2001ModInstancesGenerator : IInstancesGenerator
    {
        public IIndexedEnumerable<IClasInstance> Generate(EMR emr, CorefChainCollection groundTruth)
        {
            var instances = new List<IClasInstance>();
            var concepts = emr.Concepts;

            for (int j = concepts.Count - 1; j >= 1; j--)
            {
                var ana = concepts[j];

                if (ana.Type == ConceptType.Pronoun)
                {
                    instances.Add(new PronounInstance(ana));
                }
                else
                {
                    for (int i = j - 1; i >= 0; i--)
                    {
                        var ante = concepts[i];
                        if (ana.Type == ante.Type)
                        {
                            instances.Add(PairInstance.Create(ante, ana));
                            if (groundTruth.IsCoref(ante, ana))
                            {
                                break;
                            }
                        }
                    }
                }

                if (ana.Type == ConceptType.Person)
                {
                    instances.Add(new PersonInstance(ana));
                }
            }

            return instances.ToIndexedEnumerable();
        }
    }
}
