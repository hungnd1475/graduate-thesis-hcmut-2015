using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class WeInformationFeature : Feature
    {
        public WeInformationFeature(PersonPair instance)
            : base("We-Information", new double[2])
        {
            Value = (string.Equals(instance.Anaphora.Lexicon.ToLower(), "we") &&
                    string.Equals(instance.Antecedent.Lexicon.ToLower(), "we")) ?
                new[] { 0d, 1d } : new[] { 1d, 0d };
        }
    }
}
