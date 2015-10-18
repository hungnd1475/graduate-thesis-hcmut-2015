using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class SubstringFeature : Feature
    {
        public SubstringFeature(IConceptPair instance)
            : base("Substring-Feature", 2, 0)
        {
            var anaArr = instance.Anaphora.Lexicon.Split(' ');
            var anteArr = instance.Antecedent.Lexicon.Split(' ');

            if (anaArr.Intersect(anteArr).Any())
            {
                SetCategoricalValue(1);
            }
        }
    }
}
