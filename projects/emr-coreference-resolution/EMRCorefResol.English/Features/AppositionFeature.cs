using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    using Utilities;

    class AppositionFeature : Feature
    {
        public AppositionFeature(IConceptPair instance, EMR emr, double mentionDistance)
            : base("Apposition", 2, 0)
        {
            // checks if the two concepts are in a same sentence and separated by a comma
            if (mentionDistance == 0d)
            {
                var s = emr.ContentBetween(instance.Antecedent, instance.Anaphora);
                if (string.Equals(s, ",") || string.Equals(s, " , ") 
                    || string.Equals(s, " ,") || string.Equals(s, ", "))
                {
                    SetCategoricalValue(1);
                }
            }
        }

        public AppositionFeature(ISingleConcept instance, EMR emr)
            :base("Apposition", 2, 0)
        {
            if(instance.Concept.Begin.WordIndex == 0)
            {
                return;
            }

            var line = EMRExtensions.GetLine(emr, instance.Concept.Begin.Line);
            var tokens = line.Replace("  ", " ").Replace("\r", "").Split(' ');

            if(instance.Concept.End.WordIndex >= tokens.Length - 1)
            {
                return;
            }

            var preceeded = tokens[instance.Concept.Begin.WordIndex - 1];
            var followup = tokens[instance.Concept.End.WordIndex + 1];

            if(preceeded.Equals(",") && followup.Equals(","))
            {
                SetCategoricalValue(1);
            }
        }
    }
}
