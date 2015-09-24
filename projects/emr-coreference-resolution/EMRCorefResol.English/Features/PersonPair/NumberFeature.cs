using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace HCMUT.EMRCorefResol.English.Features
{
    class NumberFeature : Feature
    {
        public NumberFeature(PersonPair instance)
            : base("Number-Information", 3, 0)
        {
            PluralizationService ps = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

            string[] plural = { "we", "they" };
            string[] single = { "i", "you", "he", "she", "it" };

            bool anaIsPlural = plural.Contains(instance.Anaphora.Lexicon.ToLower()) ||
                ps.IsPlural(instance.Anaphora.Lexicon.ToLower());

            bool anteIsPlural = plural.Contains(instance.Antecedent.Lexicon.ToLower()) ||
                ps.IsPlural(instance.Antecedent.Lexicon.ToLower());

            bool anaIsSingular = single.Contains(instance.Anaphora.Lexicon.ToLower()) ||
                ps.IsSingular(instance.Anaphora.Lexicon.ToLower());

            bool anteIsSingular = single.Contains(instance.Antecedent.Lexicon.ToLower()) ||
                ps.IsSingular(instance.Antecedent.Lexicon.ToLower());


            if (!anteIsSingular && !anteIsPlural)
            {
                SetCategoricalValue(2);
                return;
            }

            if (!anaIsSingular && !anaIsPlural)
            {
                SetCategoricalValue(2);
                return;
            }

            if (anaIsPlural && anteIsPlural)
            {
                SetCategoricalValue(1);
                return;
            }

            if (anaIsSingular && anteIsSingular)
            {
                SetCategoricalValue(1);
                return;
            }
        }
    }
}
