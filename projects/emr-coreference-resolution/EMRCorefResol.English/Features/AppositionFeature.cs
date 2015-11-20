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
        public AppositionFeature(IConceptPair instance, EMR emr, double sentenceDistance)
            : base("Apposition", 2, 0)
        {
            // checks if the two concepts are in a same sentence and separated by a comma or a space
            if (sentenceDistance == 0d)
            {
                var s = emr.ContentBetween(instance.Antecedent, instance.Anaphora);
                var commaIndex = s.IndexOf(',');

                if (commaIndex >= 0)
                {
                    var prep = EnglishNormalizer.FindProposition(s);
                    if (!string.IsNullOrEmpty(prep))
                    {
                        var prepIndex = s.IndexOf(prep);
                        if (prepIndex + prep.Length < commaIndex)
                        {
                            s = s.Remove(prepIndex, commaIndex - prepIndex);
                        }
                    }

                    if (string.Equals(s.Trim(), ","))
                    {
                        var anteNameFeat = new NameFeature(new PersonInstance(instance.Antecedent), emr);
                        var anaNameFeat = new NameFeature(new PersonInstance(instance.Anaphora), emr);

                        if (anteNameFeat.GetCategoricalValue() == 1 ^ anaNameFeat.GetCategoricalValue() == 1)
                        {
                            SetCategoricalValue(1);
                        }
                    }
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
                if (string.Equals(s.Trim(), ","))
                {
                    SetCategoricalValue(1);
                }
            }
        }
    }
}
