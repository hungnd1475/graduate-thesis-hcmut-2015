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
            // checks if the two concepts are in a same sentence and separated by a comma or a space
            if (mentionDistance == 0d)
            {
                var s = emr.ContentBetween(instance.Antecedent, instance.Anaphora);
                if (string.Equals(s, ",") || string.Equals(s, " , ")
                    || string.Equals(s, " ,") || string.Equals(s, ", ")
                    || string.Equals(s, " "))
                {
                    SetCategoricalValue(1);
                }
            }
        }

        public AppositionFeature(ISingleConcept instance, EMR emr)
            : base("Apposition", 2, 0)
        {
            var con = instance.Concept;
            var prevCon = emr.GetPrevConcept(con);
            if (prevCon != null && prevCon.Type == con.Type)
            {
                var s = emr.ContentBetween(prevCon, con);
                if (string.Equals(s, ",") || string.Equals(s, " , ")
                    || string.Equals(s, " ,") || string.Equals(s, ", ")
                    || string.Equals(s, " "))
                {
                    SetCategoricalValue(1);
                }
            }
        }
    }
}
