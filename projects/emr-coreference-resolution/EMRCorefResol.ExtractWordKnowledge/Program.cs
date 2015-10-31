using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HCMUT.EMRCorefResol.IO;
using System.IO;

namespace HCMUT.EMRCorefResol.ExtractWordKnowledge
{
    class Program
    {
        static readonly int WIKI = 0;
        static readonly int UMLS = 1;

        static void Main(string[] args)
        {
            var collection = new EMRCollection(@"..\..\..\..\..\dataset\i2b2_Test");
            BatchUMLSProcess(collection);
            //BatchWikiProcess(collection);

            Console.ReadLine();
        }

        static void ProcessFile(EMRCollection collection)
        {
            var emrPath = collection.GetEMRPath(2);
            var conceptPath = collection.GetConceptsPath(2);
            var dataReader = new I2B2DataReader();

            var emr = new EMR(emrPath, conceptPath, dataReader);
            var filename = new FileInfo(emr.Path).Name;

            Console.WriteLine($"Processing file {filename}");
            var dictionary = ExtractUMLSData(emr);
            WriteToFile(emr.Path, dictionary);
            Console.WriteLine($"Finish processing file {filename}");
        }

        static void BatchUMLSProcess(EMRCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                var emrPath = collection.GetEMRPath(i);
                var conceptPath = collection.GetConceptsPath(i);
                var dataReader = new I2B2DataReader();

                var emr = new EMR(emrPath, conceptPath, dataReader);
                var filename = new FileInfo(emr.Path).Name;

                Console.WriteLine($"Processing file {filename}");
                var dictionary = ExtractUMLSData(emr);
                WriteToFile(emr.Path, dictionary);
                Console.WriteLine($"Finish processing file {filename}");
            }
        }

        static void BatchWikiProcess(EMRCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                var emrPath = collection.GetEMRPath(i);
                var conceptPath = collection.GetConceptsPath(i);
                var dataReader = new I2B2DataReader();

                var emr = new EMR(emrPath, conceptPath, dataReader);
                var filename = new FileInfo(emr.Path).Name;

                Console.WriteLine($"Processing file {filename}");
                var dictionary = ExtractWikiData(emr);
                WriteToFile(emr.Path, dictionary);
                Console.WriteLine($"Finish processing file {filename}");
            }
        }

        static Dictionary<string, Service.WikiData> ExtractWikiData(EMR emr)
        {
            Dictionary<string, Service.WikiData> _wiki = new Dictionary<string, Service.WikiData>();

            int num = 0;
            foreach(Concept c in emr.Concepts)
            {
                Console.WriteLine($"Number of concept processed: {num}/{emr.Concepts.Count}");
                Console.SetCursorPosition(0, Console.CursorTop-1);

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
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            return _wiki;
        }

        static Dictionary<string, Service.UMLSData> ExtractUMLSData(EMR emr)
        {
            Dictionary<string, Service.UMLSData> _umls = new Dictionary<string, Service.UMLSData>();

            int num = 0;
            foreach (Concept c in emr.Concepts)
            {
                Console.WriteLine($"Number of concept processed: {num}/{emr.Concepts.Count}");
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                if(c.Type == ConceptType.Problem)
                {
                    var key = $"{c.Lexicon}|ANA";
                    if (!_umls.ContainsKey(key))
                    {
                        var normalized = EnglishNormalizer.Normalize(c.Lexicon);
                        var umlsData = Service.English.GetUMLSInformation(normalized, Service.UMLSUtil.UMLS_ANATOMY);
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
                        var normalized = EnglishNormalizer.Normalize(c.Lexicon);
                        var umlsData = Service.English.GetUMLSInformation(normalized, Service.UMLSUtil.UMLS_ANATOMY);
                        _umls.Add(key, umlsData);
                    }

                    key = $"{c.Lexicon}|EQP";
                    if (!_umls.ContainsKey(key))
                    {
                        var normalized = EnglishNormalizer.Normalize(c.Lexicon);
                        var umlsData = Service.English.GetUMLSInformation(normalized, Service.UMLSUtil.UMLS_EQUIPMENT);
                        _umls.Add(key, umlsData);
                    }
                }

                num++;
            }
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            return _umls;
        }

        static void WriteToFile(string path, Dictionary<string, Service.WikiData> dictionary)
        {
            FileInfo fileinfo = new FileInfo(path);
            var root = fileinfo.Directory.Parent.FullName;
            var emrName = fileinfo.Name;
            var filePath = Path.Combine(root, "wiki", emrName);

            StreamWriter sw = new StreamWriter(filePath);
            foreach(var entry in dictionary)
            {
                if(entry.Value != null)
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
    }
}
