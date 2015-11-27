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
            var concepts = emr.Concepts;

            var pairs = new Concept[concepts.Count][];
            Parallel.For(1, concepts.Count, i =>
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
                        pairs[i] = new[] { bestAnte, ana };
                    }
                }
                else
                {
                    var r = ClassifyInstance(new PronounInstance(ana), fExtractor, classifier);
                    if (r != null)
                    {
                        var clasInt = Convert.ToInt32(r.Class);
                        var type = (ConceptType)clasInt;

                        if (type != ConceptType.None)
                        {
                            var cloneAna = ana.Clone(type);

                            for (int j = i - 1; j >= 0; j--)
                            {
                                var ante = concepts[j];
                                if (ante.Type == cloneAna.Type)
                                {
                                    pairs[i] = new[] { ante, cloneAna };
                                    break;
                                }
                            }
                        }
                    }
                }
            });

            var chains = pairs.Aggregate(new List<HashSet<Concept>>(), (chainsList, pair) =>
            {
                if (pair != null && pair.Length > 0)
                {
                    bool doUnion = false;
                    foreach (var ch in chainsList)
                    {
                        if (ch.Contains(pair[0]) || ch.Contains(pair[1]))
                        {
                            ch.UnionWith(pair);
                            doUnion = true;
                            break;
                        }
                    }

                    if (!doUnion)
                    {
                        chainsList.Add(new HashSet<Concept>(pair));
                    }
                }

                return chainsList;
            });

            fExtractor.ClearCache();
            classifier.ClearCache();

            var resultChains = chains.Select(s => new CorefChain(s.OrderBy(c => c), s.First().Type)).ToList();
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
