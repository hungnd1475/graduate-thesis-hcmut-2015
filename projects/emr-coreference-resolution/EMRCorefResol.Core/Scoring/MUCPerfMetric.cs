using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Scoring
{
    public class MUCPerfMetric : IPerfMetric
    {
        public string Name
        {
            get { return "MUC"; }
        }

        public Dictionary<ConceptType, Evaluation> Evaluate(EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            var evals = new Dictionary<ConceptType, Evaluation>();

            foreach (var type in Evaluations.ConceptTypes)
            {
                var tSystemChains = systemChains.GetChainsOfType(type);
                var tGroundTruth = groundTruth.GetChainsOfType(type);

                double p, r;
                if (tSystemChains.Count == 0 && tGroundTruth.Count == 0)
                {
                    p = r = 1d;
                }
                else
                {
                    if (tSystemChains.Count == 0)
                    {
                        p = 0d;
                    }
                    else
                    {
                        double u = 0d, l = 0d;
                        foreach (var s in tSystemChains)
                        {
                            u += (s.Count - m(s, tGroundTruth));
                            l += (s.Count - 1);
                        }
                        p = u / l;
                    }

                    if (tGroundTruth.Count == 0)
                    {
                        r = 0d;
                    }
                    else
                    {
                        double u = 0d, l = 0d;
                        foreach (var g in tGroundTruth)
                        {
                            u += (g.Count - m(g, tSystemChains));
                            l += (g.Count - 1);
                        }
                        r = u / l;
                    }
                }  

                var f = (p == 0 && r == 0) ? 0d : 2 * p * r / (p + r);
                evals.Add(type, new Evaluation(p, r, f, Name));
            }
            return evals;
        }

        private int m(CorefChain chain, CorefChainCollection chainsColl)
        {
            var overlap = new HashSet<Concept>();
            var r = 0;

            foreach (var c in chainsColl)
            {
                var o = c.Intersect(chain);
                if (o.Count > 0)
                {
                    r += 1;
                    overlap.UnionWith(o);
                }
            }

            var remain = chain.Except(overlap);
            r += remain.Count;

            return r;
        }
    }
}
