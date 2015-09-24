using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PronounTheyFeature : Feature
    {
        public PronounTheyFeature(PersonInstance instance)
            :base("Pronoun-They", 2, 0)
        {
            var v = (checkContain(instance.Concept.Lexicon.ToLower(), "they") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "their") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "them") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "themselves")) ?
                1 : 0;
            SetCategoricalValue(v);
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
