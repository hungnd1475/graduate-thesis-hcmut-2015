using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PronounIFeature : Feature
    {
        public PronounIFeature(PersonInstance instance)
            : base("Prounoun-I", 2, 0)
        {
            var v = (checkContain(instance.Concept.Lexicon.ToLower(), "i") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "me") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "my") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "myself")) ?
                1 : 0;
            SetCategoricalValue(v);
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
