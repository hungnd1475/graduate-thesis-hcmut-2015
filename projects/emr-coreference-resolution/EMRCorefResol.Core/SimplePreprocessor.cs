using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol 
{
    public class SimplePreprocessor : IPreprocessor
    {
        public IIndexedEnumerable<IClasInstance> Process(EMR emr)
        {
            List<IClasInstance> instances = new List<IClasInstance>();

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
                            switch (ante.Type)
                            {
                                case ConceptType.Person:
                                    instances.Add(new PersonPair(ante, ana));
                                    break;
                                case ConceptType.Problem:
                                    instances.Add(new ProblemPair(ante, ana));
                                    break;
                                case ConceptType.Test:
                                    instances.Add(new TestPair(ante, ana));
                                    break;
                                case ConceptType.Treatment:
                                    instances.Add(new TreatmentPair(ante, ana));
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
    }
}
