using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Utilities;
using System.IO;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;

namespace HCMUT.EMRCorefResol.English
{
    class KeywordService
    {
        public static KeywordService Instance { get; private set; } = null;

        private const string KWPath = "Keywords";

        public IKeywordDictionary I_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "i.txt")));

        public IKeywordDictionary YOU_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "you.txt")));

        public IKeywordDictionary WE_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "we.txt")));

        public IKeywordDictionary THEY_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "they.txt")));

        public IKeywordDictionary PRONOUNS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "pronouns.txt")));

        public IKeywordDictionary DOCTOR_TITLES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "doctor-titles.txt")));

        public IKeywordDictionary DOCTOR_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "doctors.txt")));

        public IKeywordDictionary GENERAL_DOCTOR { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "general-doctors.txt")));

        public IKeywordDictionary PATIENT_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "patients.txt")));

        public IKeywordDictionary RELATIVES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "relatives.txt")));

        private KeywordService() { }

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

        public static void LoadKeywords()
        {
            if (Instance == null)
            {
                GetLogger().WriteInfo("Loading keywords...");
                Instance = new KeywordService();
            }
        }
    }
}
