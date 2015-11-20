using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    /// <summary>
    /// Concept is a name and appeared before in EMR
    /// </summary>
    class WhoFeatures : Feature
    {
        public WhoFeatures(IConceptPair instance, EMR emr)
            : base("Who", 2, 0)
        {
            var s = emr.ContentBetween(instance);

            if (string.Equals(s.Trim(), ":"))
            {
                SetCategoricalValue(1);
            }
            else if (string.Equals(instance.Anaphora.Lexicon, "who") 
                && (string.Equals(s, " ") || string.Equals(s.Trim(), ",")))
            {
                SetCategoricalValue(1);
            }
        }

        public WhoFeatures(ISingleConcept instance, EMR emr)
            : base("Who", 2, 0)
        {
            var con = instance.Concept;
            var prevCon = emr.GetPrevConcept(con);
            if (prevCon != null && prevCon.Type == con.Type)
            {
                var s = emr.ContentBetween(prevCon, con);
                if (string.Equals(s.Trim(), ":"))
                {
                    SetCategoricalValue(1);
                }
                else if (string.Equals(instance.Concept.Lexicon, "who")
                    && (string.Equals(s, " ") || string.Equals(s.Trim(), ",")))
                {
                    SetCategoricalValue(1);
                }
            }
        }
    }
}
