using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public class MUCPerfMetric : IPerfMetric
    {
        public Evaluation Evaluate(EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            var p = systemChains.Aggregate(0d, (t, s) => t + (double)(s.Count - m(s, groundTruth)) / (s.Count - 1));
            var r = groundTruth.Aggregate(0d, (t, g) => t + (double)(g.Count - m(g, systemChains)) / (g.Count - 1));

            var f = 2 * p * r / (p + r);
            return new Evaluation(p, r, f);
        }

        private int m(CorefChain chain, CorefChainCollection chainsColl)
        {
            return chainsColl.Aggregate(0, (r, c) => c.Intersect(chain).Any() ? r + 1 : r);
        }
    }
}
