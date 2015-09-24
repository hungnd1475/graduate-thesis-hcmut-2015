using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class NameMatchFeature : Feature
    {
        public NameMatchFeature(PersonPair instance)
            : base("Name-Match", 2, 0)
        {
            string anaphora = instance.Anaphora.Lexicon;
            string antecedent = instance.Antecedent.Lexicon;

            anaphora = removeStopWords(anaphora);
            antecedent = removeStopWords(antecedent);

            if (checkContain(anaphora, antecedent))
            {
                SetCategoricalValue(1);
            }
        }

        private string removeStopWords(string raw)
        {
            string[] stopwords = { "mr", "mr.", "mrs", "mrs.", "ms", "ms.", "dr", "dr.",
                                   "md", "m.d", "m.d.", "md.",
                                   "phd", "phd.", "prof", "prof."
                                };

            string[] nameArr = raw.ToLower().Split(' ');

            List<string> tmp = new List<string>();
            for (int i = 0; i < nameArr.Length; i++)
            {
                if (!stopwords.Contains(nameArr[i]))
                {
                    tmp.Add(nameArr[i].Replace("'", "").Replace("\"", "").Replace(":", ""));
                }
            }
            return String.Join(" ", tmp);
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2))) ||
                Regex.IsMatch(s2, string.Format(@"\b{0}\b", Regex.Escape(s1)));
        }
    }
}
