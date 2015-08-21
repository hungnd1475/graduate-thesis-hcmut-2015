using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HCMUT.EMRCorefResol
{
    public class EMR
    {
        private static Regex ConceptPattern = new Regex("c=\"(?<c>.+)\" (?<begin>\\d+:\\d+) (?<end>\\d+:\\d+)");
        private static Regex TypePattern = new Regex("t=\"(.+)\"");

        public string Content { get; }

        public IIndexedEnumerable<Concept> Concepts { get; }

        public EMR(string emrFile, string conceptsFile)
        {
            var fs = new FileStream(emrFile, FileMode.Open);
            var sr = new StreamReader(fs);
            Content = sr.ReadToEnd();
            sr.Close();

            fs = new FileStream(conceptsFile, FileMode.Open);
            sr = new StreamReader(fs);
            List<Concept> concepts = new List<Concept>();
            
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var matchConcept = ConceptPattern.Match(line);
                var matchType = TypePattern.Match(line);

                if (matchConcept.Success && matchType.Success)
                {
                    var c = matchConcept.Groups["c"].Value;
                    var begin = ConceptPosition.Parse(matchConcept.Groups["begin"].Value);
                    var end = ConceptPosition.Parse(matchConcept.Groups["end"].Value);
                    var type = (ConceptType) Enum.Parse(typeof(ConceptType), matchType.Groups[1].Value, true);
                    concepts.Add(new Concept(c, begin, end, type));
                }
            }
            sr.Close();

            Concepts = concepts.OrderBy(c => c.Begin).ToIndexedEnumerable();
        }
    }
}
