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
            var tokens = Service.English.getTokens(line);

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
            var nameArr = Service.English.getTokens(name);

            if(nameArr == null)
            {
                return;
            }

            if (Char.IsUpper(nameArr.First()[0]) && Char.IsUpper(nameArr.Last()[0]))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
