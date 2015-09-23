using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class LevenshteinDistanceFeature : Feature
    {
        public LevenshteinDistanceFeature(IConceptPair instance)
            : base("Levenshtein-Distance", () =>
            {
                string anaLex = instance.Anaphora.Lexicon, anteLex = instance.Antecedent.Lexicon;
                var lev = new int[anteLex.Length + 1, anaLex.Length + 1];

                for (int k = 0; k < anteLex.Length + 1; k++)
                    lev[k, 0] = k;

                for (int k = 0; k < anaLex.Length + 1; k++)
                    lev[0, k] = k;

                for (int i = 1; i < anteLex.Length + 1; i++)
                {
                    for (int j = 1; j < anaLex.Length + 1; j++)
                    {
                        var dif = (anteLex[i - 1] == anaLex[j - 1]) ? 0 : 1;
                        lev[i, j] = Math.Min(lev[i - 1, j - 1] + dif, Math.Min(lev[i - 1, j] + 1, lev[i, j - 1] + 1));
                    }
                }

                return new double[] { lev[anteLex.Length, anaLex.Length] };
            })
        {
            //string anaLex = instance.Anaphora.Lexicon, anteLex = instance.Antecedent.Lexicon;
            //var lev = new int[anteLex.Length + 1, anaLex.Length + 1];

            //for (int k = 0; k < anteLex.Length + 1; k++)
            //    lev[k, 0] = k;

            //for (int k = 0; k < anaLex.Length + 1; k++)
            //    lev[0, k] = k;

            //for (int i = 1; i < anteLex.Length + 1; i++)
            //{
            //    for (int j = 1; j < anaLex.Length + 1; j++)
            //    {
            //        var dif = (anteLex[i - 1] == anaLex[j - 1]) ? 0 : 1;
            //        lev[i, j] = Math.Min(lev[i - 1, j - 1] + dif, Math.Min(lev[i - 1, j] + 1, lev[i, j - 1] + 1));
            //    }
            //}

            //Value = lev[anteLex.Length, anaLex.Length];
        }
    }
}
