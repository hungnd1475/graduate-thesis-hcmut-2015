using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Scoring
{
    public class BCubedPerfMetric : IPerfMetric
    {
        public string Name
        {
            get { return "BCubed"; }
        }

        public Dictionary<ConceptType, Evaluation> Evaluate(EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            var precisions = new Dictionary<ConceptType, double>();
            var recalls = new Dictionary<ConceptType, double>();
            var mCounts = new Dictionary<ConceptType, int>();

            precisions[ConceptType.None] = 0d;
            recalls[ConceptType.None] = 0d;
            mCounts[ConceptType.None] = 0;

            foreach (var m in emr.Concepts)
            {
                var g = groundTruth.FindChainContains(m);
                var s = systemChains.FindChainContains(m);

                double p, r;
                mCounts[ConceptType.None] += 1;

                if (g == null && s == null) // system not produce any chains for singleton
                {
                    precisions[ConceptType.None] += 1;
                    recalls[ConceptType.None] += 1;
                }
                else
                {
                    ConceptType type;

                    if (g == null) // system produces a chain for singleton
                    {
                        type = s.Type;
                        p = 1 / s.Count;
                        r = 1;
                    }
                    else if (s == null) // system does not produce any chains for mention m
                    {
                        type = g.Type;
                        p = 1;
                        r = 1 / g.Count;
                    }
                    else // system produces a chain for mention m
                    {
                        type = g.Type;
                        var o = g.Intersect(s);
                        p = ((double)o.Count / s.Count);
                        r = ((double)o.Count / g.Count);
                    }

                    if (!mCounts.ContainsKey(type))
                    {
                        mCounts[type] = 0;
                    }

                    if (!precisions.ContainsKey(type))
                    {
                        precisions[type] = 0d;
                    }

                    if (!recalls.ContainsKey(type))
                    {
                        recalls[type] = 0d;
                    }

                    mCounts[type] += 1;

                    precisions[type] += p;
                    recalls[type] += r;

                    precisions[ConceptType.None] += p;
                    recalls[ConceptType.None] += r;
                }
            }

            var evals = new Dictionary<ConceptType, Evaluation>();
            foreach (var t in Evaluations.ConceptTypes)
            {
                var p = precisions.ContainsKey(t) ? precisions[t] / mCounts[t] : 1d;
                var r = recalls.ContainsKey(t) ? recalls[t] / mCounts[t] : 1d;
                var f = (p == 0 && r == 0) ? 0d : 2 * p * r / (p + r);
                evals.Add(t, new Evaluation(p, r, f, Name));
            }

            return evals;
        }
    }
}
