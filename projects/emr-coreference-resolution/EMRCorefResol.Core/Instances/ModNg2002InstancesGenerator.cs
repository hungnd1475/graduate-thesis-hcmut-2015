using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class ModNg2002InstancesGenerator : IInstancesGenerator
    {
        public IIndexedEnumerable<IClasInstance> Generate(EMR emr, CorefChainCollection groundTruth)
        {
            var instances = new List<IClasInstance>();
            var concepts = emr.Concepts;

            for (int j = concepts.Count - 1; j >= 0; j--)
            {
                var ana = concepts[j];

                if (ana.Type == ConceptType.Pronoun)
                {
                    instances.Add(new PronounInstance(ana));
                }
                else if (ana.Type == ConceptType.Person)
                {
                    instances.Add(new PersonInstance(ana));
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
            }

            for (int i = 0; i < concepts.Count; i++)
            {
                var ante = concepts[i];
                if (ante.Type != ConceptType.Pronoun && ante.Type != ConceptType.Person)
                {
                    for (int j = i + 1; j < concepts.Count; j++)
                    {
                        var ana = concepts[j];
                        if (ana.Type == ante.Type && groundTruth.IsCoref(ante, ana))
                        {
                            instances.Add(PairInstance.Create(ante, ana));
                            for (int k = i + 1; k < j; k++)
                            {
                                var negAnte = concepts[k];
                                if (negAnte.Type == ana.Type)
                                {
                                    instances.Add(PairInstance.Create(negAnte, ana));
                                }
                            }
                        }
                    }
                }
            }

            return instances.ToIndexedEnumerable();
        }
    }
}
