using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class YouInformationFeature : Feature
    {
        public YouInformationFeature(PersonPair instance)
            : base("You-Information", new double[2])
        {
            Value = (string.Equals(instance.Anaphora.Lexicon.ToLower(), "you") &&
                    string.Equals(instance.Antecedent.Lexicon.ToLower(), "you")) ?
                new[] { 0d, 1d } : new[] { 1d, 0d };
        }
    }
}
