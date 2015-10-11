using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.IO
{
    public class I2B2DataReader : IDataReader
    {
        private static Regex ConceptPattern = new Regex("c=\"(?<c>[^|]+)\" (?<begin>\\d+:\\d+) (?<end>\\d+:\\d+)");
        private static Regex ConceptTypePattern = new Regex("t=\"([^|]+)\"");
        private static Regex CorefTypePattern = new Regex("t=\"coref ([^|]+)\"");

        public IEnumerable<Concept> ReadMultiple(string line)
        {
            var matchConcept = ConceptPattern.Match(line);
            var matchCorefType = CorefTypePattern.Match(line);
            var matchType = ConceptTypePattern.Match(line);

            while (matchConcept.Success)
            {
                yield return ReadConcept(matchConcept, matchCorefType, matchType);
                matchConcept = matchConcept.NextMatch();
            }
        }

        public Concept ReadSingle(string line)
        {
            var matchConcept = ConceptPattern.Match(line);
            var matchType = ConceptTypePattern.Match(line);
            var matchCoref = CorefTypePattern.Match(line);
            return matchConcept.Success ? ReadConcept(matchConcept, matchCoref, matchType) : null;
        }

        public ConceptType ReadType(string line)
        {
            var matchType = ConceptTypePattern.Match(line);
            var matchCoref = CorefTypePattern.Match(line);
            return ReadType(matchCoref, matchType);
        }

        public List<EMRSection> ReadSection(string EMRContent)
        {
            var sections = new List<EMRSection>();
            var lines = EMRContent.Replace("\r", "").Split('\n');

            var content = "";
            var title = "";
            var begin = 1;
            var end = 1;
            for(int i=0; i<lines.Length; i++)
            {
                var line = lines[i];
                if(line.Length > 0 && char.IsUpper(line[0]) && line[line.Length -1]== ':')
                {
                    title = line;
                    begin = i + 1;

                    while (true)
                    {
                        i++;
                        var nextLine = lines[i];
                        if (i == lines.Length -1 || (nextLine.Length > 0 && char.IsUpper(nextLine[0]) && nextLine[nextLine.Length - 1] == ':'))
                        {
                            i--;
                            end = i + 1;
                            var section = new EMRSection(title, content, begin, end);
                            sections.Add(section);
                            content = "";
                            break;
                        }
                        content += nextLine + "\n";
                    }
                }
            }

            return sections;
        }

        private Concept ReadConcept(Match matchConcept, Match matchCoref, Match matchType)
        {
            var c = matchConcept.Groups["c"].Value;
            var begin = ConceptPosition.Parse(matchConcept.Groups["begin"].Value);
            var end = ConceptPosition.Parse(matchConcept.Groups["end"].Value);
            var type = ReadType(matchCoref, matchType);
            return new Concept(c, begin, end, type);
        }

        private ConceptType ReadType(Match matchCoref, Match matchType)
        {
            return matchCoref.Success ? ConceptTypeHelper.Parse(matchCoref.Groups[1].Value, true) :
                (matchType.Success ? ConceptTypeHelper.Parse(matchType.Groups[1].Value, true) : ConceptType.None);
        }
    }
}
