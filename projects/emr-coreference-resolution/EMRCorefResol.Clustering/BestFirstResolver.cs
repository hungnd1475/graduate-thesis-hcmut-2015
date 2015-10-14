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
            //Console.WriteLine("Resolving...");

            fExtractor.EMR = emr;
            var concepts = emr.Concepts.ToList();
            var chains = new List<HashSet<Concept>>();

            for (int i = concepts.Count - 1; i >= 0; i--)
            {
                var ana = concepts[i];
                //Console.Write($"{ana}||t=\"{ana.Type.ToString().ToLower()}\" ");

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

                        if (r != null && r.Class == 1d)
                        {
                            if (bestResult == null || bestResult.Confidence < r.Confidence)
                            {
                                bestResult = r;
                                bestAnte = ante;
                            }                            
                        }
                    }
                    
                    if (bestResult != null && bestAnte != null)
                    {
                        //Console.Write($"-> {bestAnte}:{bestResult.Confidence}");

                        var containedChains = chains.Where(c => c.Contains(bestAnte) || c.Contains(ana)).ToArray();
                        if (containedChains.Length == 0)
                        {
                            var ch = new HashSet<Concept>() { ana, bestAnte };
                            chains.Add(ch);
                        }
                        else if (containedChains.Length == 1)
                        {
                            var ch = containedChains[0];
                            ch.Add(ana);
                            ch.Add(bestAnte);
                        }
                        else
                        {
                            var unionChain = new HashSet<Concept>();
                            foreach (var ch in containedChains)
                            {
                                chains.Remove(ch);
                                unionChain.UnionWith(ch);
                            }
                            chains.Add(unionChain);
                        }
                    }

                    //Console.WriteLine();     
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
