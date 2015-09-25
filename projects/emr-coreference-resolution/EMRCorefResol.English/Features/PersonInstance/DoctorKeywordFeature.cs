using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DoctorKeywordFeature : Feature
    {
        public DoctorKeywordFeature(PersonInstance instance)
            :base("Doctor-Keyword", 2, 0)
        {
            string[] stopWords = { "dr", "d.r.", "d.r", "dds",
                "d.d.s.", "d.d.s", "dmd", "d.m.d.",
                "d.m.d", "do", "d.o.", "d.o",
                "dpm", "d.p.m.", "d.p.m", "faaem",
                "f.a.a.e.m.", "f.a.a.e.m", "faafp", "f.a.a.f.p.",
                "f.a.a.f.p", "facc", "f.a.c.c.", "f.a.c.c",
                "face", "f.a.c.e.", "f.a.c.e", "facep",
                "f.a.c.e.p.", "f.a.c.e.p", "facg", "f.a.c.g.",
                "f.a.c.g", "facfas", "f.a.c.f.a.s.", "f.a.c.f.a.s",
                "facog", "f.a.c.o.g.", "f.a.c.o.g", "facos",
                "f.a.c.o.s.", "f.a.c.o.s", "facp", "f.a.c.p.",
                "f.a.c.p", "faccp", "f.a.c.c.p.", "f.a.c.c.p",
                "facs", "f.a.c.s.", "f.a.c.s", "fasps",
                "f.a.s.p.s.", "f.a.s.p.s", "fhm", "f.h.m.",
                "f.h.m", "fics", "f.i.c.s.", "f.i.c.s",
                "fscai", "f.s.c.a.i.", "f.s.c.a.i", "fsts",
                "f.s.t.s.", "f.s.t.s", "md", "m.d.", "m.d", "mph", "m.p.h.",
                "m.p.h", "np", "n.p.", "n.p", "od",
                "o.d.", "o.d", "pa", "p.a.", "p.a", "phd", "ph.d.", "ph.d", };

            foreach (string stopWord in stopWords)
            {
                if (checkContain(instance.Concept.Lexicon.ToLower(), stopWord))
                {
                    SetCategoricalValue(1);
                    return;
                }
            }
        }

        private bool checkContain(string s1, string s2)
        {
            return Regex.IsMatch(s1, string.Format(@"\b{0}\b", Regex.Escape(s2)));
        }
    }
}
