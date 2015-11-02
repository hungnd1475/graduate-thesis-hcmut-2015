using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class AllInstancesGenerator : IInstancesGenerator
    {
        public IIndexedEnumerable<IClasInstance> Generate(EMR emr, CorefChainCollection groundTruth)
        {
            var instances = new List<IClasInstance>();

            for (int i = 0; i < emr.Concepts.Count; i++)
            {
                var ante = emr.Concepts[i];
                if (ante.Type == ConceptType.Pronoun)
                {
                    instances.Add(new PronounInstance(ante));
                }
                else
                {
                    for (int j = i + 1; j < emr.Concepts.Count; j++)
                    {
                        var ana = emr.Concepts[j];
                        if (ante.Type == ana.Type)
                        {
                            instances.Add(PairInstance.Create(ante, ana));
                        }
                    }
                }

                if (ante.Type == ConceptType.Person)
                {
                    instances.Add(new PersonInstance(ante));
                }
            }

            return instances.ToIndexedEnumerable();
        }
    }
}
