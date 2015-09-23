using HCMUT.EMRCorefResol.English.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DoctorGeneralMatch : Feature
    {
        public DoctorGeneralMatch(PersonPair instance)
            : base("Doctor-General-Match", () =>
            {
                var drGenerals = Settings.Default.GeneralDoctors;
                var anteLex = instance.Antecedent.Lexicon.ToLower();
                var anaLex = instance.Anaphora.Lexicon.ToLower();
                string kw = null;

                foreach (var dg in drGenerals)
                {
                    if (anteLex.Contains(dg))
                    {
                        kw = dg;
                        break;
                    }
                }

                return (kw != null && anaLex.Contains(kw)) ? new[] { 0d, 1d } : new[] { 1d, 0d };
            })
        { }
    }
}
