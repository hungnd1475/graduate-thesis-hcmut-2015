using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public class BCubePerfMetric : IPerfMetric
    {
        public Evaluation Evaluate(EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            double p = 0d, r = 0d;
            foreach (var m in emr.Concepts)
            {
                var g = groundTruth.FindChainContains(m);
                var s = systemChains.FindChainContains(m);
                
                if (g != null && s != null)
                {
                    var o = g.Intersect(s);
                    p += (double)o.Count / s.Count;
                    r += (double)o.Count / g.Count;
                }
            }
            var f = 2 * p * r / (p + r);
            return new Evaluation(p, r, f);
        }
    }
}
