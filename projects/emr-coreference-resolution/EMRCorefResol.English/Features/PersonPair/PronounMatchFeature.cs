using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class PronounMatchFeature : Feature
    {
        public PronounMatchFeature(PersonPair instance, EMR emr)
            :base("Pronoun-Match", 2, 0)
        {
            var searcher = KeywordService.Instance.PERSON_PRONOUN;
            if(!searcher.Match(instance.Anaphora.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) ||
                !searcher.Match(instance.Antecedent.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            {
                return;
            }

            if(appearBefore(instance.Anaphora, emr) && appearBefore(instance.Antecedent, emr))
            {
                SetCategoricalValue(1);
            }
        }

        private bool appearBefore(Concept concept, EMR emr)
        {
            var index = emr.Concepts.IndexOf(concept);
            foreach(Concept c in emr.Concepts)
            {
                if(emr.Concepts.IndexOf(c) >= index)
                {
                    return false;
                }

                var s1 = new AhoCorasickKeywordDictionary(new string[] { c.Lexicon });
                var s2 = new AhoCorasickKeywordDictionary(new string[] { concept.Lexicon });
                if(s1.Match(concept.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord) ||
                    s2.Match(c.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
