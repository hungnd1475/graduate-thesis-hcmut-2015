using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Utilities;
using System.IO;

namespace HCMUT.EMRCorefResol.English
{
    public class KeywordService
    {
        public static KeywordService Instance { get; private set; } = null;

        private const string KWPath = "Keywords";

        public IKeywordDictionary I_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "i.txt")));

        public IKeywordDictionary YOU_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "you.txt")));

        public IKeywordDictionary HESHE_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "heshe.txt")));

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
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "general-doctor.txt")));

        public IKeywordDictionary PATIENT_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "patients.txt")));

        public IKeywordDictionary RELATIVES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "relatives.txt")));

        public IKeywordDictionary DEPARTMENT_KEYWORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "department.txt")));

        public IKeywordDictionary FEMALE_NAMES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "female-names.txt")));

        public IKeywordDictionary FEMALE_TITLES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "female-titles.txt")));

        public IKeywordDictionary MALE_NAMES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "male-names.txt")));

        public IKeywordDictionary MALE_TITLES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "male-titles.txt")));

        public IKeywordDictionary GENERAL_DEPARTMENT { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "general-department.txt")));

        public IKeywordDictionary GENERAL_TITLES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "general-titles.txt")));

        public IKeywordDictionary SIGN_INFORMATION { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "sign-information.txt")));

        public IKeywordDictionary TWIN_TRIPLET { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "twin-triplet.txt")));

        public IKeywordDictionary PERSON_PRONOUN { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "person-pronoun.txt")));

        public IKeywordDictionary NLINE_KEYWORD { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "nline-keywords.txt")));

        public IKeywordDictionary POSITION_KEYWORD { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "position-keywords.txt")));

        public IKeywordDictionary INDICATOR_KEYWORD { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "indicator-keywords.txt")));

        public IKeywordDictionary MODIFIER_KEYWORD { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "modifier-keywords.txt")));

        public IKeywordDictionary STOP_WORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "stopwords.txt")));

        public IKeywordDictionary SECTION_TITLES { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "section-titles.txt")));

        public IKeywordDictionary PERSON_BEFORE_WORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "person-before.txt")));

        public IKeywordDictionary PERSON_AFTER_WORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "person-after.txt")));

        public IKeywordDictionary PRONOUN_BEFORE_WORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "pronoun-before.txt")));

        public IKeywordDictionary PRONOUN_AFTER_WORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "pronoun-after.txt")));

        public IKeywordDictionary VERBS_AFTER_WORDS { get; }
            = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "verbs-after.txt")));

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
                Console.WriteLine("Loading keywords...");
                Instance = new KeywordService();
            }
        }
    }
}
