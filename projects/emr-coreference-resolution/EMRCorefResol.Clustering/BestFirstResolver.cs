using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;

namespace HCMUT.EMRCorefResol.CorefResolvers
{
    public class BestFirstResolver : ICorefResolver
    {
        public CorefChainCollection Resolve(EMR emr, IFeatureExtractor fExtractor, IClassifier classifier)
        {
            fExtractor.EMR = emr;
            var concepts = emr.Concepts.ToList();
            var chains = new List<HashSet<Concept>>();

            for (int i = concepts.Count - 1; i >= 0; i--)
            {
                var ana = concepts[i];

                if (ana.Type != ConceptType.Pronoun)
                {
                    ClasResult bestResult = null;
                    Concept bestAnte = null;

                    for (int j = i - 1; j >= 0; j--)
                    {
                        var ante = concepts[j];
                        if (ante.Type != ana.Type)
                            continue;

                        var r = Classify(ante, ana, fExtractor, classifier);

                        if (r.Class == 1d)
                        {
                            if (bestResult == null || (r != null && bestResult.Confidence < r.Confidence))
                            {
                                bestResult = r;
                                bestAnte = ante;
                            }
                        }
                    }
                    
                    if (bestResult != null && bestAnte != null)
                    {
                        var ch = chains.FirstOrDefault(t => t.Contains(ana) || t.Contains(bestAnte));
                        if (ch == null)
                        {
                            ch = new HashSet<Concept>();
                            chains.Add(ch);
                        }

                        ch.Add(ana);
                        ch.Add(bestAnte);
                    }                    
                }
            }

            var resultChains = new List<CorefChain>();
            foreach (var ch in chains)
            {
                var t = ch.OrderBy(c => c);
                var ante = t.First();
                resultChains.Add(new CorefChain(t, ante, ante.Type));
            }

            return new CorefChainCollection(resultChains);              
        }

        private ClasResult Classify(Concept ante, Concept ana, IFeatureExtractor fExtractor, IClassifier classifier)
        {
            switch (ante.Type)
            {
                case ConceptType.Person:
                    {
                        var ins = new PersonPair(ante, ana);
                        var feat = fExtractor.Extract(ins);
                        return classifier.Classify(ins, feat);
                    }
                default:
                    return null;
            }
        }
    }
}
