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
        public NameMatchFeature(PersonPair instance, EMR emr)
            : base("Name-Match", 2, 0)
        {
            var anaName = new NameFeature(new PersonInstance(instance.Anaphora), emr);
            var anteName = new NameFeature(new PersonInstance(instance.Antecedent), emr);

            if(anaName.Value[0] == 1 || anteName.Value[0] == 1)
            {
                return;
            }

            var seacher = KeywordService.Instance.GENERAL_TITLES;
            string anaNorm = seacher.RemoveKeywords(instance.Anaphora.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            string anteNorm = seacher.RemoveKeywords(instance.Antecedent.Lexicon, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (anaNorm.Contains(anteNorm) || anteNorm.Contains(anaNorm))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
