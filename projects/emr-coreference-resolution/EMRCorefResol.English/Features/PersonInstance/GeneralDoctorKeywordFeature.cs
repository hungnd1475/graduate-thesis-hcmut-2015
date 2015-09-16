using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class GeneralDoctorKeywordFeature : Feature
    {
        public GeneralDoctorKeywordFeature(PersonInstance instance)
            : base("GeneralDoctor-Keyword")
        {
            string[] stopWords = { "doctor", "pcp", "author", "dict", "attend", "provider" };
            foreach (string stopWord in stopWords)
            {
                if (checkContain(instance.Concept.Lexicon.ToLower(), stopWord))
                {
                    Value = 1.0;
                    return;
                }
            }
            Value = 0.0;
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
