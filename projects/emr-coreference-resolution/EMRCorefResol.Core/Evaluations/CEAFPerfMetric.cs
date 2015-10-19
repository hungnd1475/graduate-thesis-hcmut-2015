using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public class CEAFPerfMetric : IPerfMetric
    {
        public Evaluation Evaluate(EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {            
            var c1 = groundTruth;
            var c2 = systemChains;
            if (c1.Count > c2.Count)
            {
                GenericHelper.Swap(ref c1, ref c2);
            }

            double bestPhi = 0d;
            findBestPhi(c1, c2, 0, new MapItem[c1.Count], new HashSet<int>(), ref bestPhi);

            var p = bestPhi / systemChains.Aggregate(0d, (t, s) => t + phi4(s, s));
            var r = bestPhi / groundTruth.Aggregate(0d, (t, g) => t + phi4(g, g));
            var f = 2 * p * r / (p + r);

            return new Evaluation(p, r, f);                                                                                                                 
        }

        private double phi4(CorefChain a, CorefChain b)
        {
            var o = a.Intersect(b);
            return 2 * o.Count / (a.Count + b.Count);
        }

        private void findBestPhi(CorefChainCollection c1, CorefChainCollection c2, int i, 
            MapItem[] map, HashSet<int> mapped, ref double bestPhi)
        {
            for (int j = 0; j < c2.Count; j++)
            {
                if (c1[i].Type != c2[j].Type)
                    continue;

                if (!mapped.Contains(j))
                {
                    map[i] = new MapItem(c1[i], c2[j]);
                    mapped.Add(j);
                    
                    if (i < c1.Count - 1)
                    {
                        findBestPhi(c1, c2, i + 1, map, mapped, ref bestPhi);
                    }
                    else
                    {
                        var phi = map.Aggregate(0d, (p, m) => p + phi4(m.Chain1, m.Chain2));
                        if (phi > bestPhi)
                        {
                            bestPhi = phi;
                        }
                    }

                    mapped.Remove(j);
                }
            }
        }

        struct MapItem
        {
            public CorefChain Chain1 { get; }
            public CorefChain Chain2 { get; }

            public MapItem(CorefChain chain1, CorefChain chain2)
            {
                Chain1 = chain1;
                Chain2 = chain2;
            }
        }
    }
}
