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
                    for (int j = i + 1; j < concepts.Count; j++)
                    {
                        var ana = concepts[j];

                        if (ante.Type == ana.Type)
                        {
                            if (groundTruth.IsCoref(ante, ana))
                            {
                                var inst = PairInstance(ante, ana);
                                if (inst != null)
                                {
                                    instances.Add(inst);
                                }

                                for (int k = i + 1; k < j; k++)
                                {
                                    var c = concepts[k];
                                    inst = PairInstance(c, ana);
                                    if (inst != null)
                                    {
                                        instances.Add(inst);
                                    }
                                }

                                break;
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

        static IClasInstance PairInstance(Concept ante, Concept ana)
        {
            switch (ante.Type)
            {
                case ConceptType.Person:
                    return new PersonPair(ante, ana);
                case ConceptType.Problem:
                    return new ProblemPair(ante, ana);
                case ConceptType.Test:
                    return new TestPair(ante, ana);
                case ConceptType.Treatment:
                    return new TreatmentPair(ante, ana);
                default:
                    return null;
            }
        }
    }
}
