using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class NumberFeature : Feature
    {
        /// <summary>
        /// Set value = 0 if not both are singular or plural
        /// Set value = 1 if both are singular or plural
        /// Set value = 2 if one is undefine
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="emr"></param>
        public NumberFeature(PersonPair instance, EMR emr)
            : base("Number-Information", 3, 2)
        {
            var anaForm = getForm(instance.Anaphora, emr);
            var anteForm = getForm(instance.Antecedent, emr);

            if(anteForm == 2 || anaForm == 2)
            {
                return;
            }

            if(anaForm == 0 && anteForm == 0)
            {
                SetCategoricalValue(1);
                return;
            }

            if (anaForm == 1 && anteForm == 1)
            {
                SetCategoricalValue(1);
                return;
            }

            SetCategoricalValue(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="emr"></param>
        /// <returns>
        /// 0 if singular
        /// 1 if plural
        /// 2 if undefined
        /// </returns>
        private int getForm(Concept c, EMR emr)
        {
            var single = new AhoCorasickKeywordDictionary(new string[] { "i", "my", "you", "your", "he", "his", "she", "her", "patient" });
            var relative = KeywordService.Instance.RELATIVES;
            var isName = new NameFeature(new PersonInstance(c), emr);

            if(single.Match(c.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) ||
                relative.Match(c.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) ||
                isName.GetCategoricalValue() == 1)
            {
                return 0;
            }

            var plural = new AhoCorasickKeywordDictionary(new string[] { "we", "they" });
            if(plural.Match(c.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                return 1;
            }

            var pos = Service.English.POSTag(c.Lexicon);
            if (pos != null)
            {
                var tag = pos[0].Split('|')[1];
                if(tag.Equals("NN", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 0;
                } else if (tag.Equals("NNS", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 1;
                }
            }

            return 2;
        }
    }
}
