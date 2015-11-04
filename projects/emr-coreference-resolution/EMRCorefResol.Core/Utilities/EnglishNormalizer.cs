using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    using Utilities;
    public class EnglishNormalizer
    {
        private const string KWPath = @"..\..\..\EMRCorefResol.English\Keywords";

        public static string Normalize(string term)
        {
            var searcher = new AhoCorasickKeywordDictionary(ReadKWFile(Path.Combine(KWPath, "stopwords.txt")));
            var normalized = searcher.RemoveKeywords(term, KWSearchOptions.WholeWordIgnoreCase);

            return string.IsNullOrEmpty(normalized) ? term : RemovePreposition(normalized);
        }

        public static string FindProposition(string term)
        {
            var chunks = Service.English.GetChunks(term);

            if (chunks != null)
            {
                foreach (string chunk in chunks)
                {
                    var t = chunk.Split('|')[0];
                    var compoundTag = chunk.Split('|')[1];

                    if (!compoundTag.Equals("O", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var tag = compoundTag.Split('-')[1];
                        if (tag.Equals("PP", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return t;
                        }
                    }
                }
            }

            return null;
        }

        public static string RemovePreposition(string term)
        {
            var preposition = FindProposition(term);
            if (preposition == null)
            {
                return term;
            }

            var index = term.IndexOf(preposition);

            if (index.Equals(0))
            {
                return term.Substring(preposition.Length + 1, term.Length - preposition.Length - 1);
            }
            else
            {
                return term.Substring(0, index - 1);
            }
        }

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
    }
}
