using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class PronounIndexFeature : Feature
    {
        public PronounIndexFeature(PronounInstance instance)
            : base("Pronoun-Index", 18, 0)
        {
            string[] pronounList = { "it", "they", "them", "that",
                "which", "what", "who", "whom",
                "whose", "all", "any", "most",
                "some", "this", "that", "these",
                "those" };

            if (pronounList.Contains(instance.Concept.Lexicon.ToLower()))
            {
                SetCategoricalValue(Array.IndexOf(pronounList, instance.Concept.Lexicon.ToLower()) + 1);
            }
        }
    }
}
