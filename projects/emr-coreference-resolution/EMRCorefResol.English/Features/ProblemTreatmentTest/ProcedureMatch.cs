using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class ProcedureMatch : Feature
    {
        public ProcedureMatch(IConceptPair instance)
            :base("Procedure-Match", 2, 0)
        {
            if ((instance.Anaphora.Lexicon.Contains("procedure") || instance.Anaphora.Lexicon.Contains("procedures")) &&
                (instance.Antecedent.Lexicon.Contains("procedure") || instance.Antecedent.Lexicon.Contains("procedures")))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
