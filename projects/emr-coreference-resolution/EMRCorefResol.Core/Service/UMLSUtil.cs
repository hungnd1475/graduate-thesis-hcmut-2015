using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace HCMUT.EMRCorefResol.Service
{
    public class UMLSUtil
    {
        public static int UMLS_ANATOMY = 0;
        public static int UMLS_EQUIPMENT = 1;
        public static int UMLS_OPERATION = 2;
        public static int UMLS_INDICATOR = 3;

        private const string UMLS_ROOT = @"E:\public_mm\bin\metamap";

        public UMLSData GetUMLSInfo(string term)
        {
            var rawText = RunBatCommand(term);
            XmlDocument xmlDoc = ParseXML(rawText);
            if (xmlDoc == null) return null;

            var bestResult = GetBestResult(xmlDoc);
            var UMLSData = ParseUMLSData(bestResult);

            return UMLSData;
        }

        public UMLSData GetUMLSInfo(string term, int restrict)
        {
            var options = "";
            switch (restrict)
            {
                case 0:
                    options = "-J \"anst,blor,bpoc,bsoj,bdsu,bdsy\"";
                    break;
                case 1:
                    options = "-J \"medd,diap\"";
                    break;
                case 2:
                    options = "-J topp";
                    break;
                case 3:
                    options = "-J \"qnco,phsu,elii,lbtr,chem,inch,orch,lbpr\"";
                    break;
            }

            var rawText = RunBatCommand(term, options);
            XmlDocument xmlDoc = ParseXML(rawText);
            if (xmlDoc == null) return null;

            var bestResult = GetBestResult(xmlDoc);
            var UMLSData = ParseUMLSData(bestResult);

            return UMLSData;
        }

        private string RunBatCommand(string term)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.Arguments = $"/c echo \"{term}\" | {UMLS_ROOT} --XMLf --silent";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        private string RunBatCommand(string term, string options)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.Arguments = $"/c echo \"{term}\" | {UMLS_ROOT} --XMLf --silent {options}";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        private XmlDocument ParseXML(string rawText)
        {
            var regex = new Regex("\\<Mappings Count=\"\\d\"\\>[\\s\\S]*?\\<\\/Mappings\\>");
            Match xml = regex.Match(rawText);

            if (xml.Success)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml.Value);
                return xmlDoc;
            }

            return null;
        }

        private XmlNode GetBestResult(XmlDocument xmlDoc)
        {
            var mappings = xmlDoc.GetElementsByTagName("Mapping");

            var bestScore = 0;
            XmlNode bestResult = null;
            foreach(XmlNode mapping in mappings)
            {
                var score = -int.Parse(mapping["MappingScore"].InnerText);
                if(score > bestScore)
                {
                    bestScore = score;
                    bestResult = mapping;
                }
            }

            return bestResult;
        }

        private UMLSData ParseUMLSData(XmlNode xml)
        {
            var bestCandidate = GetBestCandidate(xml);
            if (bestCandidate == null) return null;

            var concept = bestCandidate["CandidateMatched"].InnerText;
            var prefer = bestCandidate["CandidatePreferred"].InnerText;
            var cui = bestCandidate["CandidateCUI"].InnerText;
            var score = -int.Parse(bestCandidate["CandidateScore"].InnerText);

            List<string> semantics = new List<string>();
            var semtypes = bestCandidate["SemTypes"].GetElementsByTagName("SemType");
            foreach(XmlNode semtype in semtypes)
            {
                semantics.Add(semtype.InnerText);
            }

            return new UMLSData(cui, concept, prefer, semantics.ToArray(), score);
        }

        private XmlNode GetBestCandidate(XmlNode xml)
        {
            var candidates = xml["MappingCandidates"].GetElementsByTagName("Candidate");

            var bestScore = 0;
            XmlNode bestCandidate = null;
            foreach(XmlNode node in candidates)
            {
                var candidateScore = -int.Parse(node["CandidateScore"].InnerText);

                if (candidateScore > bestScore)
                {
                    bestScore = candidateScore;
                    bestCandidate = node;
                }
            }

            return bestCandidate;
        }
    }
}
