using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;
    class GenderFeature : Feature
    {
        public GenderFeature(PersonPair instance, EMR emr)
            : base("Gender-Information", 3, 0)
        {
            int anaGen = getGender(instance.Anaphora.Lexicon, emr);
            int anteGen = getGender(instance.Antecedent.Lexicon, emr);

            if (anaGen == 2 || anteGen == 2)
            {
                SetCategoricalValue(2);
            }
            else if (anaGen == anteGen)
            {
                SetCategoricalValue(1);
            }
        }

        private int getGender(string name, EMR emr)
        {
            int keyword = containKeyword(name);
            if(keyword != 2)
            {
                return keyword;
            }

            var appeared = appearedBefore(name, emr);
            if(appeared != 2)
            {
                return appeared;
            }

            var fromDB = getGender(name);
            if(fromDB != 2)
            {
                return fromDB;
            }

            return 2;
        }

        private int containKeyword(string name)
        {
            var s = new AhoCorasickKeywordDictionary(new string[] { "father", "brother", "he", "him", "himself", "husband", "son", "uncle", "nephew", "dad" });
            if (s.Match(name, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return 0;
            }

            s = new AhoCorasickKeywordDictionary(new string[] { "mother", "sister", "she", "her", "herself", "wife", "daughter", "aunt", "niece", "mom" });
            if (s.Match(name, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return 1;
            }

            var searcher = KeywordService.Instance.MALE_TITLES;
            if(searcher.Match(name, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return 0;
            }

            searcher = KeywordService.Instance.FEMALE_TITLES;
            if (searcher.Match(name, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return 1;
            }

            return 2;
        }

        private int appearedBefore(string name, EMR emr)
        {
            foreach(Concept c in emr.Concepts)
            {
                if(c.Type == ConceptType.Person)
                {
                    var nameArr = name.Split(' ');
                    var conceptArr = c.Lexicon.Split(' ');

                    if(conceptArr.Intersect(nameArr).Count() == 0)
                    {
                        continue;
                    }

                    var keyword = containKeyword(c.Lexicon);
                    if (keyword == 2)
                    {
                        continue;
                    }

                    return keyword;
                }
            }
            return 2;
        }

        private int getGender(string name)
        {
            var searcher = KeywordService.Instance.DOCTOR_KEYWORDS;
            var nameNormal = searcher.RemoveKeywords(name, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            if(Service.English.Tokenize(nameNormal) == null)
            {
                return 2;
            }
            nameNormal = Service.English.Tokenize(nameNormal)[0];

            searcher = KeywordService.Instance.MALE_NAMES;
            if(searcher.Match(nameNormal, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return 0;
            }

            searcher = KeywordService.Instance.FEMALE_NAMES;
            if (searcher.Match(nameNormal, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return 1;
            }

            return 2;
        }
    }
}
