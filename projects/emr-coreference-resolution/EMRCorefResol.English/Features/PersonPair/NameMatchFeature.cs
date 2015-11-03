using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class NameMatchFeature : Feature
    {
        static readonly IKeywordDictionary STOP_WORDS =
            new AhoCorasickKeywordDictionary("m", "m.", ",", ":");

        public NameMatchFeature(PersonPair instance, EMR emr)
            : base("Name-Match", 2, 0)
        {
            var anaName = new NameFeature(new PersonInstance(instance.Anaphora), emr);
            var anteName = new NameFeature(new PersonInstance(instance.Antecedent), emr);

            if (anaName.GetCategoricalValue() == 0 || anteName.GetCategoricalValue() == 0)
            {
                return;
            }

            var seacher = KeywordService.Instance.GENERAL_TITLES;
            string anaNorm = seacher.RemoveKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWordIgnoreCase);
            string anteNorm = seacher.RemoveKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWordIgnoreCase);

            anaNorm = STOP_WORDS.RemoveKeywords(anaNorm, KWSearchOptions.WholeWordIgnoreCase).Trim();
            anteNorm = STOP_WORDS.RemoveKeywords(anteNorm, KWSearchOptions.WholeWordIgnoreCase).Trim();

            var anaArr = anaNorm.Split(' ');
            var anteArr = anteNorm.Split(' ');

            if (anteArr.Intersect(anaArr).Any())
            {
                SetCategoricalValue(1);
            }
        }
    }
}
