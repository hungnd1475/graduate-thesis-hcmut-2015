using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class Soon2001InstancesGenerator : IInstancesGenerator
    {
        public IIndexedEnumerable<IClasInstance> Generate(EMR emr, CorefChainCollection groundTruth)
        {
            var instances = new List<IClasInstance>();
            var concepts = emr.Concepts;

            for (int i = 0; i < concepts.Count; i++)
            {
                var ante = concepts[i];
                if (ante.Type == ConceptType.Pronoun)
                {
                    instances.Add(new PronounInstance(ante));
                }
                else
                {
                    if (groundTruth.IsSingleton(ante))
                    {
                        for (int j = i + 1; j < concepts.Count; j++)
                        {
                            var ana = concepts[j];
                            if (ante.Type == ana.Type && groundTruth.IsSingleton(ana))
                            {
                                instances.Add(PairInstance.Create(ante, ana));
                            }
                        }
                    }
                    else
                    {
                        for (int j = i + 1; j < concepts.Count; j++)
                        {
                            var ana = concepts[j];

                            if (ante.Type == ana.Type)
                            {
                                if (groundTruth.IsCoref(ante, ana))
                                {
                                    instances.Add(PairInstance.Create(ante, ana));

                                    for (int k = i + 1; k < j; k++)
                                    {
                                        var c = concepts[k];
                                        if (c.Type == ana.Type)
                                        {
                                            instances.Add(PairInstance.Create(c, ana));
                                        }
                                    }

                                    break;
                                }
                            }
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
