using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HCMUT.EMRCorefResol.IO;
using System.IO;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol.ExtractWordKnowledge
{
    class Program
    {
        const string KWPath = @"D:\Documents\Visual Studio 2015\Projects\graduate-thesis-hcmut-2015\projects\emr-coreference-resolution\EMRCorefResol.English\Keywords";

        public static IKeywordDictionary PATIENT_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "patients.txt")));

        public static IKeywordDictionary RELATIVES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "relatives.txt")));

        private static IEnumerable<string> ReadKWFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    yield return sr.ReadLine();
                }
            }
        }

        static void Main(string[] args)
        {
            var collection = new EMRCollection(@"..\..\..\..\..\dataset\i2b2_Train");
            //BatchUMLSProcess(collection);
            //BatchWikiProcess(collection);
            //BatchTemporalProcess(collection);
            BatchExtractWordsPerson(collection);
            BatchExtractWordsPronoun(collection);

            collection = new EMRCollection(@"..\..\..\..\..\dataset\i2b2_Test");
            //BatchUMLSProcess(collection);
            //BatchWikiProcess(collection);
            //BatchTemporalProcess(collection);

            Console.WriteLine("========Finish========");
            Console.ReadLine();
        }

        static void ProcessFile(EMRCollection collection)
        {
            var emrPath = collection.GetEMRPath(2);
            var conceptPath = collection.GetConceptsPath(2);
            var dataReader = new I2B2DataReader();

            var emr = new EMR(emrPath, conceptPath, dataReader);
            var filename = new FileInfo(emr.Path).Name;

            var dictionary = ExtractUMLSData(emr);
            WriteToFile(emr.Path, dictionary);
            Console.WriteLine($"Finish processing file {filename}");
        }

        static void BatchUMLSProcess(EMRCollection collection)
        {
            Parallel.For(0, collection.Count,
                i =>
                {
                    var emrPath = collection.GetEMRPath(i);
                    var conceptPath = collection.GetConceptsPath(i);
                    var dataReader = new I2B2DataReader();

                    var emr = new EMR(emrPath, conceptPath, dataReader);
                    var filename = new FileInfo(emr.Path).Name;

                    var dictionary = ExtractUMLSData(emr);
                    WriteToFile(emr.Path, dictionary);
                    Console.WriteLine($"Finish processing file {filename}");
                });
        }

        static void BatchTemporalProcess(EMRCollection collection)
        {
            Parallel.For(0, collection.Count,
                i =>
                {
                    var emrPath = collection.GetEMRPath(i);
                    var conceptPath = collection.GetConceptsPath(i);
                    var dataReader = new I2B2DataReader();

                    var emr = new EMR(emrPath, conceptPath, dataReader);
                    var filename = new FileInfo(emr.Path).Name;

                    var dictionary = ExtractTemporalData(emr);
                    WriteToFile(emr.Path, dictionary);
                    Console.WriteLine($"Finish processing file {filename}");
                });
        }

        static void BatchWikiProcess(EMRCollection collection)
        {
            Parallel.For(0, collection.Count,
                i =>
                {
                    var emrPath = collection.GetEMRPath(i);
                    var conceptPath = collection.GetConceptsPath(i);
                    var dataReader = new I2B2DataReader();

                    var emr = new EMR(emrPath, conceptPath, dataReader);
                    var filename = new FileInfo(emr.Path).Name;

                    var dictionary = ExtractWikiData(emr);
                    WriteToFile(emr.Path, dictionary);
                    Console.WriteLine($"Finish processing file {filename}");
                });
        }

        static Dictionary<string, string> ExtractTemporalData(EMR emr)
        {
            Dictionary<string, string> _temporal = new Dictionary<string, string>();
            var lines = emr.Content.Split('\n');

            foreach (string line in lines)
            {
                var fullPath = new FileInfo(emr.Path).FullName;
                var normLine = line.Replace("\n", "").Replace("\r", "");
                var temporalValue = Service.English.GetTemporalValue(fullPath, normLine, "");
                if (!string.IsNullOrEmpty(temporalValue) && !_temporal.ContainsKey(normLine))
                {
                    _temporal.Add(normLine, temporalValue);
                }
            }

            return _temporal;
        }

        static Dictionary<string, Service.WikiData> ExtractWikiData(EMR emr)
        {
            Dictionary<string, Service.WikiData> _wiki = new Dictionary<string, Service.WikiData>();

            int num = 0;
            foreach (Concept c in emr.Concepts)
            {
                if (c.Type == ConceptType.Problem || c.Type == ConceptType.Treatment || c.Type == ConceptType.Test)
                {
                    if (!_wiki.ContainsKey(c.Lexicon))
                    {
                        var normalized = EnglishNormalizer.Normalize(c.Lexicon);
                        var wikiData = Service.English.GetAllWikiInformation(normalized);
                        _wiki.Add(c.Lexicon, wikiData);
                    }
                }

                num++;
            }
            return _wiki;
        }

        static Dictionary<string, Service.UMLSData> ExtractUMLSData(EMR emr)
        {
            Dictionary<string, Service.UMLSData> _umls = new Dictionary<string, Service.UMLSData>();

            int num = 0;
            foreach (Concept c in emr.Concepts)
            {
                if (c.Type == ConceptType.Problem)
                {
                    var key = $"{c.Lexicon}|ANA";
                    if (!_umls.ContainsKey(key))
                    {
                        var umlsData = Service.English.GetUMLSInformation(c.Lexicon, Service.UMLSUtil.UMLS_ANATOMY);
                        _umls.Add(key, umlsData);
                    }
                }

                if (c.Type == ConceptType.Treatment)
                {
                    var key = $"{c.Lexicon}|OPE";
                    if (!_umls.ContainsKey(key))
                    {
                        var normalized = EnglishNormalizer.Normalize(c.Lexicon);
                        var umlsData = Service.English.GetUMLSInformation(normalized, Service.UMLSUtil.UMLS_OPERATION);
                        _umls.Add(key, umlsData);
                    }
                }

                if (c.Type == ConceptType.Test)
                {
                    var key = $"{c.Lexicon}|ANA";
                    if (!_umls.ContainsKey(key))
                    {
                        var umlsData = Service.English.GetUMLSInformation(c.Lexicon, Service.UMLSUtil.UMLS_ANATOMY);
                        _umls.Add(key, umlsData);
                    }

                    key = $"{c.Lexicon}|EQP";
                    if (!_umls.ContainsKey(key))
                    {
                        var normalized = EnglishNormalizer.Normalize(c.Lexicon);
                        var umlsData = Service.English.GetUMLSInformation(normalized, Service.UMLSUtil.UMLS_EQUIPMENT);
                        _umls.Add(key, umlsData);
                    }

                    key = $"{c.Lexicon}|INDI";
                    if (!_umls.ContainsKey(key))
                    {
                        var umlsData = Service.English.GetUMLSInformation(c.Lexicon, Service.UMLSUtil.UMLS_INDICATOR);
                        _umls.Add(key, umlsData);
                    }
                }

                num++;
            }
            return _umls;
        }

        static void WriteToFile(string path, Dictionary<string, string> dictionary)
        {
            FileInfo fileinfo = new FileInfo(path);
            var root = fileinfo.Directory.Parent.FullName;
            var emrName = fileinfo.Name;
            var filePath = Path.Combine(root, "temporal", emrName);

            StreamWriter sw = new StreamWriter(filePath);
            foreach (var entry in dictionary)
            {
                if (entry.Value != null)
                {
                    var line = $"rawTerm=\"{entry.Key}\"||temporal=\"{entry.Value}\"";
                    sw.WriteLine(line);
                    sw.Flush();
                }
            }
        }

        static void WriteToFile(string path, Dictionary<string, Service.WikiData> dictionary)
        {
            FileInfo fileinfo = new FileInfo(path);
            var root = fileinfo.Directory.Parent.FullName;
            var emrName = fileinfo.Name;
            var filePath = Path.Combine(root, "wiki", emrName);

            StreamWriter sw = new StreamWriter(filePath);
            foreach (var entry in dictionary)
            {
                if (entry.Value != null)
                {
                    var line = $"rawTerm=\"{entry.Key}\"||{entry.Value.ToString()}";
                    sw.WriteLine(line);
                    sw.Flush();
                }
            }
        }

        static void WriteToFile(string path, Dictionary<string, Service.UMLSData> dictionary)
        {
            FileInfo fileinfo = new FileInfo(path);
            var root = fileinfo.Directory.Parent.FullName;
            var emrName = fileinfo.Name;
            var filePath = Path.Combine(root, "umls", emrName);

            StreamWriter sw = new StreamWriter(filePath);
            foreach (var entry in dictionary)
            {
                if (entry.Value != null)
                {
                    var line = $"rawTerm=\"{entry.Key}\"||{entry.Value.ToString()}";
                    sw.WriteLine(line);
                    sw.Flush();
                }
            }
            sw.Close();
        }

        static void BatchExtractWordsPronoun(EMRCollection emrColl)
        {
            var before = new HashSet<string>[emrColl.Count];
            var after = new HashSet<string>[emrColl.Count];

            Parallel.For(0, emrColl.Count,
                i =>
                {
                    var emrPath = emrColl.GetEMRPath(i);
                    var conceptsPath = emrColl.GetConceptsPath(i);
                    var emr = new EMR(emrPath, conceptsPath, new I2B2DataReader());

                    before[i] = ExtractWords(emr, IsPronoun, (e, c) => GetNWordsNearBy(e, c, 3, true));
                    after[i] = ExtractWords(emr, IsPronoun, (e, c) => GetNWordsNearBy(e, c, 3, false));
                });

            var saveDir = @"D:\Documents\Visual Studio 2015\Projects\graduate-thesis-hcmut-2015\projects\emr-coreference-resolution\EMRCorefResol.English\Keywords";
            Parallel.Invoke(
                () => WriteWords(before, Path.Combine(saveDir, "pronoun-before.txt")),
                () => WriteWords(after, Path.Combine(saveDir, "pronoun-after.txt"))
            );
        }

        static void BatchExtractWordsPerson(EMRCollection emrColl)
        {
            var before = new HashSet<string>[emrColl.Count];
            var after = new HashSet<string>[emrColl.Count];

            var firstBetweenProblem = new HashSet<string>[emrColl.Count];
            var lastBetweenProblem = new HashSet<string>[emrColl.Count];

            var firstBetweenTreatment = new HashSet<string>[emrColl.Count];
            var lastBetweenTreatment = new HashSet<string>[emrColl.Count];

            var firstBetweenTest = new HashSet<string>[emrColl.Count];
            var lastBetweenTest = new HashSet<string>[emrColl.Count];

            Parallel.For(0, emrColl.Count,
                i =>
                {
                    var emrPath = emrColl.GetEMRPath(i);
                    var conceptsPath = emrColl.GetConceptsPath(i);
                    var emr = new EMR(emrPath, conceptsPath, new I2B2DataReader());

                    var gt = new CorefChainCollection(emrColl.GetChainsPath(i), new I2B2DataReader());
                    Func<Concept, bool> isPatient = c => IsPatient(c, gt);

                    before[i] = ExtractWords(emr, isPatient, (e, c) => GetNWordsNearBy(e, c, 3, true));
                    after[i] = ExtractWords(emr, isPatient, (e, c) => GetNWordsNearBy(e, c, 3, false));

                    firstBetweenProblem[i] = ExtractWords(emr, isPatient,
                        (e, c1) => GetFirstOneTwoThreeWordsBetween(e, c1, c2 => c2.Type == ConceptType.Problem));
                    lastBetweenProblem[i] = ExtractWords(emr, isPatient,
                        (e, c1) => GetLastOneTwoThreeWordsBetween(e, c1, c2 => c2.Type == ConceptType.Problem));

                    firstBetweenTest[i] = ExtractWords(emr, isPatient,
                        (e, c1) => GetFirstOneTwoThreeWordsBetween(e, c1, c2 => c2.Type == ConceptType.Test));
                    lastBetweenTest[i] = ExtractWords(emr, isPatient,
                        (e, c1) => GetLastOneTwoThreeWordsBetween(e, c1, c2 => c2.Type == ConceptType.Test));

                    firstBetweenTreatment[i] = ExtractWords(emr, isPatient,
                        (e, c1) => GetFirstOneTwoThreeWordsBetween(e, c1, c2 => c2.Type == ConceptType.Treatment));
                    lastBetweenTreatment[i] = ExtractWords(emr, isPatient,
                        (e, c1) => GetLastOneTwoThreeWordsBetween(e, c1, c2 => c2.Type == ConceptType.Treatment));
                });

            var saveDir = @"D:\Documents\Visual Studio 2015\Projects\graduate-thesis-hcmut-2015\projects\emr-coreference-resolution\EMRCorefResol.English\Keywords";
            Parallel.Invoke(
                () => WriteWords(before, Path.Combine(saveDir, "person-before.txt")),
                () => WriteWords(after, Path.Combine(saveDir, "person-after.txt")),

                () => WriteWords(firstBetweenProblem, Path.Combine(saveDir, "first-between-problem.txt")),
                () => WriteWords(lastBetweenProblem, Path.Combine(saveDir, "last-between-problem.txt")),

                () => WriteWords(firstBetweenTest, Path.Combine(saveDir, "first-between-test.txt")),
                () => WriteWords(lastBetweenTest, Path.Combine(saveDir, "last-between-test.txt")),

                () => WriteWords(firstBetweenTreatment, Path.Combine(saveDir, "first-between-treatment.txt")),
                () => WriteWords(lastBetweenTreatment, Path.Combine(saveDir, "last-between-treatment.txt"))
            );
        }

        static void WriteWords(IEnumerable<HashSet<string>> values, string filePath)
        {
            using (var sr = new StreamWriter(filePath))
            {
                var words = new HashSet<string>();
                foreach (var s in values)
                {
                    foreach (var w in s)
                    {
                        if (!words.Contains(w))
                        {
                            sr.WriteLine(w);
                            words.Add(w);
                        }
                    }
                }
            }
        }

        static HashSet<string> ExtractWords(EMR emr, Func<Concept, bool> isInterest,
            Func<EMR, Concept, HashSet<string>> getNearByWords)
        {
            var words = new HashSet<string>();
            foreach (var c in emr.Concepts)
            {
                if (isInterest(c))
                {
                    var s = getNearByWords(emr, c);
                    words.UnionWith(s);
                }
            }
            return words;
        }

        static bool IsPatient(Concept c, CorefChainCollection groundTruth)
        {
            //var ptChain = groundTruth.GetPatientChain(PATIENT_KEYWORDS, RELATIVES);
            //return ptChain?.Contains(c) ?? false;
            return c.Type == ConceptType.Person;
        }

        static bool IsPronoun(Concept c)
        {
            return c.Type == ConceptType.Pronoun;
        }

        static HashSet<string> GetNWordsNearBy(EMR e, Concept c, int n, bool left)
        {
            var words = new HashSet<string>();

            var lineNumber = left ? c.Begin.Line : c.End.Line;
            var line = removeNewLine(e.GetLine(lineNumber)).Trim();
            var tokens = line.Split(' ');

            var step = left ? -1 : 1;
            var begin = left ? c.Begin.WordIndex : c.End.WordIndex;

            for (int i = 1; i < tokens.Length && words.Count < n; i++)
            {
                var wi = begin + i * step;
                if (wi >= 0 && wi < tokens.Length)
                {
                    var w = tokens[wi].ToLower();
                    if (!isStopChar(w))
                        words.Add(w);
                }
                else
                {
                    break;
                }
            }

            return words;
        }

        static HashSet<string> GetFirstOneTwoThreeWordsBetween(EMR emr, Concept c1, Func<Concept, bool> isInterest)
        {
            var c1Index = emr.Concepts.IndexOf(c1);
            var words = new HashSet<string>();

            for (int i = c1Index + 1; i < emr.Concepts.Count; i++)
            {
                var c2 = emr.Concepts[i];
                if (isInterest(c2))
                {
                    var s = removeNewLine(emr.ContentBetween(c1, c2)).Trim();
                    var tokens = s.Split(' ');

                    for (int j = 0; j < tokens.Length && words.Count < 3; j++)
                    {
                        var w = tokens[j].ToLower();
                        if (!isStopChar(w))
                            words.Add(w);
                    }

                    break;
                }
            }

            return words;
        }

        static HashSet<string> GetLastOneTwoThreeWordsBetween(EMR emr, Concept c1, Func<Concept, bool> isInterest)
        {
            var c1Index = emr.Concepts.IndexOf(c1);
            var words = new HashSet<string>();

            for (int i = c1Index + 1; i < emr.Concepts.Count; i++)
            {
                var c2 = emr.Concepts[i];
                if (isInterest(c2))
                {
                    var s = removeNewLine(emr.ContentBetween(c1, c2)).Trim();
                    var tokens = s.Split(' ');

                    for (int j = tokens.Length - 1; j >= 0 && words.Count < 3; j--)
                    {
                        var w = tokens[j].ToLower();
                        if (!isStopChar(w))
                            words.Add(w);
                    }

                    break;
                }
            }

            return words;
        }

        static HashSet<string> STOP_CHARS = new HashSet<string>()
        {
            ",", ";", ".", ":", "-", "*", "(", ")",
            "[", "]", "!", "?", ">", "<", "\"", "'",
            "{", "}", "\\", "|", "&", "^", "%", "$",
            "#", "@", "_", "+", "=", "~", "`",
            "/"
        };

        static bool isStopChar(string s)
        {
            var c = s.Select(t => t.ToString());
            return s == string.Empty || STOP_CHARS.Contains(s) || STOP_CHARS.IsSupersetOf(c);
        }

        static bool containsNumber(string s)
        {
            var numbers = new string[]
            {
                "0", "1", "2", "3", "4", "5",
                "6", "7", "8", "9"
            };

            return numbers.Where(n => s.Contains(n)).Any();
        }

        static string removeNewLine(string s)
        {
            return s.Replace(Environment.NewLine, " ");
        }
    }
}
