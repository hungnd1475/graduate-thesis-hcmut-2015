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
            var normalized = searcher.RemoveKeywords(term, Utilities.KWSearchOptions.WholeWordIngoreCase);
            normalized = RemovePreposition(normalized);

            return normalized;
        }

        private static string FindProposition(string term)
        {
            var chunks = Service.English.GetChunks(term);
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

            return null;
        }

        private static string RemovePreposition(string term)
        {
            var preposition = FindProposition(term);
            if(preposition == null)
            {
                return term;
            }

            var index = term.IndexOf(preposition);
            return term.Substring(0, index-1);
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
