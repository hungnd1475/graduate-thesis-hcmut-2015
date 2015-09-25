using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class NameFeature : Feature
    {
        public NameFeature(PersonInstance instance, EMR emr)
            :base("Name-Feature", 2, 0)
        {
            var line = EMRExtensions.GetLine(emr, instance.Concept.Begin.Line).Split(' ');
            var rawNameArr = line.Skip(instance.Concept.Begin.WordIndex)
                .Take(instance.Concept.End.WordIndex - instance.Concept.Begin.WordIndex + 1)
                .ToArray();

            var nameArr = removeStopWords(string.Join(" ", rawNameArr)).Split(' ');

            if(Char.IsUpper(nameArr.First()[0]) && Char.IsUpper(nameArr.Last()[0]))
            {
                SetCategoricalValue(1);
            }

            int x = 1 + 1;
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
    }
}
