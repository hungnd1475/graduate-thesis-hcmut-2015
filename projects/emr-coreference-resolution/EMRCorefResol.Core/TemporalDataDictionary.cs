using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HCMUT.EMRCorefResol
{
    public class TemporalDataDictionary
    {
        private readonly List<TemporalData> _temporals = new List<TemporalData>();
        private const int MAX_PREVIOUS_LINE = 3;

        public TemporalData Get(Concept c, EMR emr)
        {
            var line = emr.GetLine(c);
            var result = GetTemporalInline(c, line, emr);

            if(result == null)
            {
                var section = emr.GetSection(c);
                result = GetTemporalInPreviousLine(line, section, emr);
            }


            return result;
        }

        private TemporalData GetTemporalInPreviousLine(string line, EMRSection section, EMR emr)
        {
            string[] sectionLines;
            if(section != null)
            {
                sectionLines = section.Content.Split('\n');
            } else
            {
                sectionLines = emr.Content.Split('\n');
            }

            for(int i=0; i<sectionLines.Length; i++)
            {
                var curLine = sectionLines[i].Replace("\r", "");
                if (curLine.Equals(line))
                {
                    int num = 1;
                    while ((i - num) >= 0 && num <= MAX_PREVIOUS_LINE)
                    {
                        var value = GetTemporalInline(null, sectionLines[i-num], emr);
                        if (value != null)
                        {
                            return value;
                        }

                        num++;
                    }

                    break;
                }
            }

            return null;
        }

        private TemporalData GetTemporalInline(Concept c, string line, EMR emr)
        {
            var lineStart = emr.Content.IndexOf(line);
            var lineEnd = lineStart + line.ToCharArray().Length - 1;

            if (lineStart == -1)
            {
                return null;
            }

            List<TemporalData> tempInline = new List<TemporalData>();
            foreach(TemporalData tempData in _temporals)
            {
                if(tempData.Start >= lineStart && tempData.End <= lineEnd)
                {
                    tempInline.Add(tempData);
                }
            }

            //Return the best Temporal data inline
            if(tempInline.Count == 0)
            {
                return null;
            } else if(tempInline.Count == 1)
            {
                return tempInline[0];
            } else
            {
                if(c == null)
                {
                    return tempInline[0];
                } else
                {
                    return GetBestResultInLine(c, line, tempInline);
                }
            }
        }

        private TemporalData GetBestResultInLine(Concept c, string line, List<TemporalData> tempInline)
        {
            TemporalData bestResult = null;
            var bestDistance = int.MaxValue;
            foreach (TemporalData temp in tempInline)
            {
                var tempBegin = line.ToLower().IndexOf(temp.Text.ToLower());
                var tempEnd = tempBegin + temp.Value.Length - 1;

                var conceptBegin = line.ToLower().IndexOf(c.Lexicon);
                var conceptEnd = conceptBegin + c.Lexicon.Length - 1;

                //2 truong` hop: Temporal nam` truoc Concept hoac Temporal nam` sau concept
                // Do vay 1 trong 2 gia tri (conceptBegin - tempEnd) hoac (tempBegin - conceptEnd) se mang gia tri am
                var distance = Math.Max(conceptBegin - tempEnd, tempBegin - conceptEnd);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestResult = temp;
                }
            }

            return bestResult;
        }

        public int Count
        {
            get { return _temporals.Count; }
        }

        public TemporalDataDictionary(string infosFile)
        {
            var content = File.ReadAllText(infosFile);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml($"<temporal>{content}</temporal>");

            var nodes = doc.GetElementsByTagName("TIMEX3");
            foreach(XmlNode node in nodes)
            {
                var start = int.Parse(node.Attributes["start"].Value);
                var end = int.Parse(node.Attributes["end"].Value);
                var text = node.Attributes["text"].Value;
                var value = node.Attributes["val"].Value;
                var temporalData = new TemporalData(start, end, text, value);

                _temporals.Add(temporalData);
            }
        }
    }
}
