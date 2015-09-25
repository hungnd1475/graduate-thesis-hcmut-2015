using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class DoctorTitleKeywordFeature : Feature
    {
        public DoctorTitleKeywordFeature(PersonInstance instance)
            : base("DoctorTitle-Keyword", 2, 0)
        {
            string[] stopWords = { "audiologist","allergist","andrologist","anesthesiologist",
                "cardiologist","cardiovascular surgeon","clinical neurophysiologis",
                "dentist","dermatologist","dmergency doctors","endocrinologist",
                "epidemiologist","ent specialist","family practitioner","gastroenterologist",
                "gynecologist","general psychiatrist","hematologist","hepatologist",
                "immunologist","infectious disease specialist","internal medicine specialist","internist",
                "medical geneticist","microbiologist","neonatologist","nephrologists",
                "neurologist","neurosurgeons","obstetrician","oncologist",
                "ophthalmologist","orthodontist","orthopedic surgeon",
                "orthopedist","primatologist","pale pathologist","parasitologist",
                "pathologist","pediatrician","plastic surgeon","physiologist",
                "physiatrist","plastic surgeon","podiatrist","psychiatrist",
                "pulmonologist","radiologist","reproductive endocrinologist","rheumatologist",
                "surgeon","thoracic oncologist","urologist","veterinarian" };

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
