using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PronounYouFeature : Feature
    {
        public PronounYouFeature(PersonInstance instance)
            : base("Pronoun-You")
        {
            Value = (checkContain(instance.Concept.Lexicon.ToLower(), "you") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "your") ||
                checkContain(instance.Concept.Lexicon.ToLower(), "yourself")) ?
                1.0 : 0.0;
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
