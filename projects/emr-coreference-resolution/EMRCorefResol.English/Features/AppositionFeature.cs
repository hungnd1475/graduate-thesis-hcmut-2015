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
            : base("Apposition")
        {
            // checks if the two concepts are in a same sentence and separated by a comma
            if (mentionDistance == 0)
            {
                var s = emr.ContentBetween(instance.Antecedent, instance.Anaphora);
                if (string.Equals(s, ",") || string.Equals(s, " , ") 
                    || string.Equals(s, " ,") || string.Equals(s, ", "))
                {
                    Value = 1.0;
                }
            }
        }
    }
}
