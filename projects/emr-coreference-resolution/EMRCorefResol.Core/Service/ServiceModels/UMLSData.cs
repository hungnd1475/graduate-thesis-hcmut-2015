using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class UMLSData
    {
        public string CUI;
        public string Concept;
        public string Prefer;
        public string[] Semantic;
        public int Confidence;

        public UMLSData(string cui, string concept, string prefer, string[] semantic, int confidence)
        {
            CUI = cui;
            Concept = concept;
            Prefer = prefer;
            Semantic = semantic;
            Confidence = confidence;
        }

        public override string ToString()
        {
            string s = string.Join("|", Semantic);
            return $"cui=\"{CUI}\"||concept=\"{Concept}\"||prefer=\"{Prefer}\"||semantic=[{s}]||confidence=\"{Confidence}\"";
        }
    }
}
