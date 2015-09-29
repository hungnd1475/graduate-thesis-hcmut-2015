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
        public NameMatchFeature(PersonPair instance)
            : base("Name-Match", 2, 0)
        {
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
