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
            var concepts = emr.Concepts;
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

                        var r = ClassifyPair(ante, ana, fExtractor, classifier);

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
                        AddToChains(chains, bestAnte, ana);                        
                    }

                    //Console.WriteLine();     
                }
                else
                {
                    var r = ClassifyInstance(new PronounInstance(ana), fExtractor, classifier);
                    var clasInt = Convert.ToInt32(r.Class);
                    var pronType = (ConceptType)clasInt;

                    for (int j = i - 1; j >= 0; j--)
                    {
                        var ante = concepts[j];
                        if (ante.Type == pronType)
                        {
                            AddToChains(chains, ante, ana);
                            break;
                        }                    
                    }
                }
            }

            var resultChains = new List<CorefChain>();
            foreach (var ch in chains)
            {
                var t = ch.OrderBy(c => c);
                var ante = t.First();
                resultChains.Add(new CorefChain(t, ante.Type));
            }

            return new CorefChainCollection(resultChains);              
        }

        private ClasResult ClassifyPair(Concept ante, Concept ana, IFeatureExtractor fExtractor, IClassifier classifier)
        {
            switch (ante.Type)
            {
                case ConceptType.Person:
                    return ClassifyInstance(new PersonPair(ante, ana), fExtractor, classifier);
                case ConceptType.Problem:
                    return ClassifyInstance(new ProblemPair(ante, ana), fExtractor, classifier);
                case ConceptType.Test:
                    return ClassifyInstance(new TestPair(ante, ana), fExtractor, classifier);
                case ConceptType.Treatment:
                    return ClassifyInstance(new TreatmentPair(ante, ana), fExtractor, classifier);
                default:
                    return null;
            }
        }

        private ClasResult ClassifyInstance<T>(T instance, IFeatureExtractor fExtractor, IClassifier classifier)
            where T : IClasInstance
        {
            var feat = instance.GetFeatures(fExtractor);
            return instance.Classify(classifier, feat);
        }

        private void AddToChains(List<HashSet<Concept>> chains, Concept ante, Concept ana)
        {
            var containedChains = chains.Where(c => c.Contains(ante) || c.Contains(ana)).ToArray();
            if (containedChains.Length == 0)
            {
                var ch = new HashSet<Concept>() { ante, ana };
                chains.Add(ch);
            }
            else if (containedChains.Length == 1)
            {
                var ch = containedChains[0];
                ch.Add(ante);
                ch.Add(ana);
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
    }
}
