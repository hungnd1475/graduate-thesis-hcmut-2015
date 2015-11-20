using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class ModifierFeature : Feature
    {
        public ModifierFeature(IConceptPair instance, EMR emr)
            :base("Modifier-Feature", 3, 2)
        {
            var searcher = KeywordService.Instance.MODIFIER_KEYWORD;

            var line = emr.GetLine(instance.Anaphora);
            var anaKeyword = searcher.SearchKeywords(line, Utilities.KWSearchOptions.IgnoreCase);
            if (anaKeyword.Length <= 0) return;

            line = emr.GetLine(instance.Antecedent);
            var anteKeyword = searcher.SearchKeywords(line, Utilities.KWSearchOptions.IgnoreCase);
            if (anteKeyword.Length <= 0) return;

            foreach(string i in anaKeyword)
            {
                foreach(string k in anteKeyword)
                {
                    var anaNormalized = i.Replace("-", "");
                    var anteNormalized = k.Replace("-", "");
                    if(anaNormalized.Equals(anteNormalized, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetCategoricalValue(1);
                    }
                }
            }

            SetCategoricalValue(0);
        }
    }
}
