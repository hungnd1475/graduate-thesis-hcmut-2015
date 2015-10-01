using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using System.Text.RegularExpressions;
    using Utilities;
    class NameFeature : Feature
    {
        public NameFeature(PersonInstance instance, EMR emr)
            :base("Name-Feature", 2, 0)
        {
            var line = EMRExtensions.GetLine(emr, instance.Concept.Begin.Line);
            if(instance.Concept.Lexicon.Split(' ').Length == 1 &&
                line.ToLower().IndexOf(instance.Concept.Lexicon) == 0)
            {
                return;
            }

            var tokens = line.Replace("  ", " ").Replace("\r", "").Split(' ');

            if (tokens == null)
            {
                return;
            }

            var rawNameArr = tokens.Skip(instance.Concept.Begin.WordIndex)
                .Take(instance.Concept.End.WordIndex - instance.Concept.Begin.WordIndex + 1)
                .ToArray();
            var rawName = string.Join(" ", rawNameArr);

            var searcher = KeywordService.Instance.GENERAL_TITLES;
            var name = searcher.RemoveKeywords(rawName, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);
            searcher = new AhoCorasickKeywordDictionary(new string[] { ",", ".", "'", "\"", "/", "\\" });
            name = searcher.RemoveKeywords(name, KWSearchOptions.WholeWord | KWSearchOptions.IgnoreCase);

            if (name == null || name.Length < 1)
            {
                return;
            }

            var nameArr = name.Split(' ');
            if (Char.IsUpper(nameArr.First()[0]) && Char.IsUpper(nameArr.Last()[0]))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
