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
        static readonly IKeywordDictionary MALE_KEYWORDS =
            new AhoCorasickKeywordDictionary("father", "brother", "he", "his", "himself", "husband", "son", "uncle", "nephew", "dad");

        static readonly IKeywordDictionary FEMALE_KEYWORDS =
            new AhoCorasickKeywordDictionary("mother", "sister", "she", "her", "herself", "wife", "daughter", "aunt", "niece", "mom");

        enum Gender
        {
            Male, Female, Unknown
        }

        public GenderFeature(PersonPair instance, EMR emr)
            : base("Gender-Information", 3, 0)
        {
            var anaGen = getGender(instance.Anaphora, emr);
            var anteGen = getGender(instance.Antecedent, emr);

            if (anaGen == Gender.Unknown || anteGen == Gender.Unknown)
            {
                SetCategoricalValue(2);
            }
            else if (anaGen == anteGen)
            {
                SetCategoricalValue(1);
            }
        }

        private Gender getGender(Concept concept, EMR emr)
        {
            var gender = Gender.Unknown;

            gender = checkKeywords(concept.Lexicon);
            if (gender != Gender.Unknown)
            {
                return gender;
            }

            gender = checkOtherMatchingConcepts(concept, emr);
            if (gender != Gender.Unknown)
            {
                return gender;
            }

            return checkCommonProperNames(concept.Lexicon);
        }

        private Gender checkKeywords(string name)
        {
            var head = Service.English.GetHeadNoun(name);

            if (head != null)
            {
                var searcher = KeywordService.Instance.MALE_TITLES;
                if (searcher.Match(head, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
                {
                    return Gender.Male;
                }

                searcher = KeywordService.Instance.FEMALE_TITLES;
                if (searcher.Match(head, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
                {
                    return Gender.Female;
                }

                if (MALE_KEYWORDS.Match(head, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
                {
                    return Gender.Male;
                }

                if (FEMALE_KEYWORDS.Match(head, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
                {
                    return Gender.Female;
                }
            }
            else
            {
                if (string.Equals(name, "her"))
                {
                    return Gender.Female;
                }
                else if (string.Equals(name, "his"))
                {
                    return Gender.Male;
                }
            }

            return Gender.Unknown;
        }

        private Gender checkOtherMatchingConcepts(Concept concept, EMR emr)
        {
            foreach (Concept c in emr.Concepts)
            {
                if (c.Type == ConceptType.Person)
                {
                    var pair = c.CompareTo(concept) < 0 ? new PersonPair(c, concept) : new PersonPair(concept, c);
                    var nameMatch = new NameMatchFeature(pair, emr);

                    if (nameMatch.GetCategoricalValue() == 1)
                    {
                        var keyword = checkKeywords(c.Lexicon);
                        if (keyword != Gender.Unknown)
                        {
                            return keyword;
                        }
                    }
                }
            }

            return Gender.Unknown;
        }

        private Gender checkCommonProperNames(string name)
        {
            var searcher = KeywordService.Instance.DOCTOR_KEYWORDS;
            var nameNormal = searcher.RemoveKeywords(name, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord);

            var tokens = Service.English.Tokenize(nameNormal);
            if (tokens == null || tokens.Length == 0)
            {
                return Gender.Unknown;
            }

            nameNormal = tokens[0];
            searcher = KeywordService.Instance.MALE_NAMES;
            if (searcher.Match(nameNormal, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return Gender.Male;
            }

            searcher = KeywordService.Instance.FEMALE_NAMES;
            if (searcher.Match(nameNormal, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase))
            {
                return Gender.Female;
            }

            return Gender.Unknown;
        }
    }
}
