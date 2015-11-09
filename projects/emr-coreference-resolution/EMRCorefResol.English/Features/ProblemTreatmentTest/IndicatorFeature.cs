using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using System.Text.RegularExpressions;
    using Utilities;
    using Service;
    class IndicatorFeature : Feature
    {
        public IndicatorFeature(IConceptPair instance, UmlsDataDictionary dictionary, EMR emr)
            :base("Indicator-Feature", 3, 2)
        {
            var anaIndi = dictionary.Get($"{instance.Anaphora.Lexicon}|INDI");
            var anaLine = emr.GetLine(instance.Anaphora);
            var anteIndi = dictionary.Get($"{instance.Antecedent.Lexicon}|INDI");
            var anteLine = emr.GetLine(instance.Antecedent);

            if (anaIndi == null || anteIndi == null)
            {
                return;
            }

            var anaValue = GetIndicatorsPairValue(anaLine, instance.Anaphora.Lexicon);
            var anteValue = GetIndicatorsPairValue(anteLine, instance.Antecedent.Lexicon);

            if(anaValue == null || anteValue == null)
            {
                return;
            }

            if(CheckMatchPair(anaValue, anteValue, anaIndi, anteIndi))
            {
                SetCategoricalValue(1);
            } else
            {
                SetCategoricalValue(0);
            }
        }

        private bool CheckMatchPair(Tuple<string, string> anaValue, Tuple<string, string> anteValue,
            UMLSData anaUmls, UMLSData anteUmls)
        {
            //Difference value
            if(anaValue.Item2 != anteValue.Item2)
            {
                return false;
            }

            //Difference indicator
            if(anaValue.Item1 != anteValue.Item1 && anaValue.Item1 != anteUmls.Concept && anaValue.Item1 != anteUmls.Prefer &&
                anteValue.Item1 != anaUmls.Concept && anteValue.Item1 != anaUmls.Prefer &&
                anteUmls.Concept != anaUmls.Concept && anteUmls.Prefer != anaUmls.Prefer &&
                anteUmls.Concept != anaUmls.Prefer && anteUmls.Prefer != anaUmls.Concept)
            {
                return false;
            }

            return true;
        }

        private Tuple<string, string> GetIndicatorsPairValue(string line, string term)
        {
            var pattern = term + "[ ]{0,}(-|of|was|were|is|are|rise to|rises to|rise|rises|rised to|rising to|down to|downed to|downs to|drop to|dropped to|drops to|drop|drops)?[ ]{0,}(\\d+\\.\\d?|\\d+,\\d?|\\d+)";
            var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
            var value = "";
            if (match.Success)
            {
                value = match.Groups[2].Value;
            }

            pattern = "(\\d+\\.\\d?|\\d+,\\d?|\\d+)[ ]{0,}(%)?[ ]{0,}" + term;
            match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                value = match.Groups[1].Value;
            }

            if (!value.Equals(""))
            {
                return Tuple.Create(term, value);
            } else
            {
                return null;
            }
        }
    }
}
