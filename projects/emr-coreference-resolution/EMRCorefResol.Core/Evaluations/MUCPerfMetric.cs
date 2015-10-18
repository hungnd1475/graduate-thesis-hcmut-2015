using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public class MUCPerfMetric : IPerfMetric
    {
        public Evaluation Evaluate(CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            var precision = CalcPrecision(groundTruth, systemChains);
            var recall = CalcRecall(groundTruth, systemChains);
            var fmeasure = 2 * precision * recall / (precision + recall);
            return new Evaluation(precision, recall, fmeasure);
        }

        private double CalcPrecision(CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            double u = 0d, l = 0d;

            foreach (var s in systemChains)
            {
                u += (s.Count - m(s, groundTruth));
                l += (s.Count - 1);
            }

            return l != 0d ? u / l : u;
        }

        private double CalcRecall(CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            double u = 0d, l = 0d;

            foreach (var g in groundTruth)
            {
                u += (g.Count - m(g, systemChains));
                l += (g.Count - 1);
            }

            return l != 0d ? u / l : u;
        }

        private int m(CorefChain chain, CorefChainCollection chainsColl)
        {
            return chainsColl.Aggregate(0, (r, c) => c.Intersect(chain).Any() ? r + 1 : r);
        }
    }
}
