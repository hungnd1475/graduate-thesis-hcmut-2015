using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class ModNg2002InstancesGenerator : IInstancesGenerator
    {
        private readonly IFilterRule _singletonFilterRule;

        public ModNg2002InstancesGenerator()
            : this(new TrueFilterRule())
        { }

        public ModNg2002InstancesGenerator(IFilterRule singletonFilterRule)
        {
            _singletonFilterRule = singletonFilterRule;
        }

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
                else
                {
                    if (groundTruth.IsSingleton(ana))
                    {
                        for (int i = 0; i < j; i++)
                        {
                            var ante = concepts[i];
                            if (ana.Type == ante.Type && groundTruth.IsSingleton(ante)
                                && _singletonFilterRule.IsSatisfied(ante, ana))
                            {
                                instances.Add(PairInstance.Create(ante, ana));
                            }
                        }
                    }
                    else
                    {
                        int fIndex = -1;
                        for (int i = 0; i < j; i++)
                        {
                            var ante = concepts[i];
                            if (groundTruth.IsCoref(ante, ana))
                            {
                                fIndex = i;
                                break;
                            }
                        }

                        if (fIndex >= 0)
                        {
                            for (int i = fIndex; i < j; i++)
                            {
                                var ante = concepts[i];
                                if (ante.Type == ana.Type)
                                {
                                    instances.Add(PairInstance.Create(ante, ana));
                                }
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
