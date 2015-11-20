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
        private static Regex UmlsDataPattern = new Regex("rawTerm=\"(.*?)\"\\|\\|cui=\"(.*?)\"\\|\\|concept=\"(.*?)\"\\|\\|prefer=\"(.*?)\"\\|\\|semantic=\\[(.*?)\\]\\|\\|confidence=\"(.*?)\"");
        private static Regex TemporalDataPattern = new Regex("rawTerm=\"(.*?)\"\\|\\|temporal=\"(.*?)\"");

        private static string _history_of_illness_pattern = "(present illness|hospitalization)(.*?):";
        private static string _allergy = "(allergies|allergy)(.*?):";
        private static string _chief_complaint = "chief complaint(.*?):";
        private static string _diagnosis = "(diagnosis|diagnoses)(.*?):";
        private static string _hospital_course = "(hospital course|course(.*?)hospital)(.*?):";
        private static string _laboratory = "(laboratory|laboratories|lab)(.*?):";
        private static string _medication = "(medication|med)(.*?):";
        private static string _past_medical = "past medical(.*?):";
        private static string _physical_exam = "physical exam(.*?):";
        private static string _followup = "(follow up|follow-up|followup)(.*?):";
        private static string _procedure = "procedure(.*?):";
        private static string _condition = "condition(.*?):";
        private static string _radiology = "(radiology|radiographic|radiologic)(.*?):";
        private static string _instruction = "discharge instruction(.*?):";
        private static string _disposition = "disposition(.*?):";


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

        public Tuple<string, string> ReadTemporalFile(string line)
        {
            var match = TemporalDataPattern.Match(line);
            if (match.Success)
            {
                var key = match.Groups[1].Value;
                var temporal = match.Groups[2].Value;

                return Tuple.Create(key, temporal);
            }
            else
            {
                return null;
            }
        }

        public Tuple<string, UMLSData> ReadUmlsFile(string line)
        {
            var match = UmlsDataPattern.Match(line);
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
            var lines = EMRContent.Split('\n');

            var content = "";
            var title = "";
            var begin = 1;
            var end = 1;
            for(int i=0; i<lines.Length; i++)
            {
                var line = lines[i].Replace("\r", "").Trim();
                var tuple = CheckHeading(line);
                //if(line.Length > 0 && char.IsUpper(line[0]) && line[line.Length -1]== ':')
                if (tuple.Item1)
                {
                    title = tuple.Item2;
                    begin = i + 1;

                    //Read section content until find a new section title
                    while (true)
                    {
                        i++;
                        if (i.Equals(lines.Length))
                        {
                            end = i;
                            var section = new EMRSection(title, content, begin, end);
                            sections.Add(section);
                            content = "";
                            break;
                        }

                        var nextLine = lines[i].Replace("\r", "").Trim();

                        tuple = CheckHeading(nextLine);
                        //if (i == lines.Length -1 || (nextLine.Length > 0 && char.IsUpper(nextLine[0]) && nextLine[nextLine.Length - 1] == ':'))
                        if (tuple.Item1)
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

        private Tuple<bool, string> CheckHeading(string line)
        {
            if(string.IsNullOrEmpty(line) || !CheckCapitol(line) || line[line.Length - 1] != ':')
            {
                return Tuple.Create(false, "");
            }

            //return Tuple.Create(true, line);

            if (Regex.IsMatch(line, _history_of_illness_pattern, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "History of Present Illness :");
            }

            if (Regex.IsMatch(line, _allergy, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Allergies :");
            }

            if (Regex.IsMatch(line, _chief_complaint, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Chief Complaint :");
            }

            if (Regex.IsMatch(line, _diagnosis, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Diagnosis :");
            }

            if (Regex.IsMatch(line, _hospital_course, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Hospital Course :");
            }

            if (Regex.IsMatch(line, _laboratory, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Laboratory Data :");
            }

            if (Regex.IsMatch(line, _past_medical, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Past Medical History :");
            }

            if (Regex.IsMatch(line, _medication, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Medication :");
            }

            if (Regex.IsMatch(line, _physical_exam, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Physical Exam :");
            }

            if (Regex.IsMatch(line, _procedure, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Procedure :");
            }

            if (Regex.IsMatch(line, _followup, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Follow-up :");
            }

            if (Regex.IsMatch(line, _condition, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Condition :");
            }

            if (Regex.IsMatch(line, _radiology, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Radiology :");
            }

            if (Regex.IsMatch(line, _instruction, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Instruction :");
            }

            if (Regex.IsMatch(line, _disposition, RegexOptions.IgnoreCase))
            {
                return Tuple.Create(true, "Disposition :");
            }

            return Tuple.Create(false, "");
        }

        private bool CheckCapitol(string title)
        {
            var words = title.Replace("\r", "").Replace("\n", "").Split(' ');

            var prepositions = new string[] { "of", "in", "at", "on" };

            foreach (string word in words)
            {
                if (string.IsNullOrEmpty(word) || word.Equals(":"))
                {
                    continue;
                }

                if (!prepositions.Contains(word) && !char.IsUpper(word[0]))
                {
                    return false;
                }
            }
            return true;
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
