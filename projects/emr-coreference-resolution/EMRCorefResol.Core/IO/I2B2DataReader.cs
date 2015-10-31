using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.IO
{
    using Service;
    public class I2B2DataReader : IDataReader
    {
        private static Regex ConceptPattern = new Regex("c=\"(?<c>[^|]+)\" (?<begin>\\d+:\\d+) (?<end>\\d+:\\d+)");
        private static Regex ConceptTypePattern = new Regex("t=\"([^|]+)\"");
        private static Regex CorefTypePattern = new Regex("t=\"coref ([^|]+)\"");

        private static Regex WikiDataPattern = new Regex("rawTerm=\"(.*?)\"\\|\\|term=\"(.*?)\"\\|\\|title=\"(.*?)\"\\|\\|links=\\[(.*?)\\]\\|\\|bolds=\\[(.*?)\\]");
        private static Regex UmlsDataPatter = new Regex("rawTerm=\"(.*?)\"\\|\\|cui=\"(.*?)\"\\|\\|concept=\"(.*?)\"\\|\\|prefer=\"(.*?)\"\\|\\|semantic=\\[(.*?)\\]\\|\\|confidence=\"(.*?)\"");

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

        public Tuple<string, WikiData> ReadWikiFile(string line)
        {
            var match = WikiDataPattern.Match(line);
            if (match.Success)
            {
                var key = match.Groups[1].Value;
                var term = match.Groups[2].Value;
                var title = match.Groups[3].Value;
                var links = match.Groups[4].Value.Split('|');
                var bolds = match.Groups[5].Value.Split('|');
                var wikiData = new WikiData(term, title, links, bolds);

                return Tuple.Create(key, wikiData);
            } else
            {
                return null;
            }
        }

        public Tuple<string, UMLSData> ReadUmlsFile(string line)
        {
            var match = UmlsDataPatter.Match(line);
            if (match.Success)
            {
                var key = match.Groups[1].Value;
                var cui = match.Groups[2].Value;
                var concept = match.Groups[3].Value;
                var prefer = match.Groups[4].Value;
                var semantic = match.Groups[5].Value.Split('|');
                var confidence = int.Parse(match.Groups[6].Value);
                var umlsData = new UMLSData(cui, concept, prefer, semantic, confidence);

                return Tuple.Create(key, umlsData);
            }
            else
            {
                return null;
            }
        }

        public MedicationInfo ReadMedInfoLine(string line)
        {
            var info_arr = line.Split('|');

            var line_info = info_arr[0].Split('\t');
            int line_index = 0;
            int.TryParse(line_info[0], out line_index);
            var line_value = line_info[1];

            var regex = @"\[(.*?)\]";
            var drug = Regex.Replace(info_arr[1], regex, "");
            var form = Regex.Replace(info_arr[3], regex, "");
            var strength = Regex.Replace(info_arr[4], regex, "");
            var dose = Regex.Replace(info_arr[5], regex, "");
            var route = Regex.Replace(info_arr[6], regex, "");
            var freq = Regex.Replace(info_arr[7], regex, "");
            var duration = Regex.Replace(info_arr[8], regex, "");
            var neccessity = Regex.Replace(info_arr[9], regex, "");

            return new MedicationInfo(line_index, line_value, drug, form, strength, dose, route,
                freq, duration, neccessity);
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

                    //Read section content until find a new section title
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
