using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PronounWeFeature : Feature
    {
        public PronounWeFeature(PersonInstance instance)
            :base("Pronoun-We", 2, 0)
        {
            var v = (checkContain(instance.Concept.Lexicon.ToLower(), "our") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "we") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "us") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "ourselves")) ?
                1 : 0;
            SetCategoricalValue(v);
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
