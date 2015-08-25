using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class CorefChain : HashSet<Concept>
    {
        public ConceptType Type { get; }

        public Concept Antecedent { get; }

        public CorefChain(Concept antecedent, ConceptType type)
        {
            Type = type;
            Antecedent = antecedent;
        }

        public override string ToString()
        {
            return string.Join("||", this.Select(c => $"{c.Lexicon} {c.Begin} {c.End}")) 
                + $"||t=\"coref {Type.ToString().ToLower()}\"";
        }
    }
}
