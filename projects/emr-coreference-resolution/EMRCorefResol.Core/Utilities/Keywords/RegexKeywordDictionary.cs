using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace HCMUT.EMRCorefResol.Utilities
{
    /// <summary>
    /// DON'T USE THIS
    /// </summary>
    public class RegexKeywordDictionary : IKeywordDictionary
    {
        private IEnumerable<string> _kwList;

        public RegexKeywordDictionary(params string[] keywords)
        {
            _kwList = keywords;
        }

        public string this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _kwList.GetEnumerator();
        }

        public bool Match(string s, KWSearchOptions options)
        {
            bool found = false;
            SearchWithAction(s, options, (kw) =>
            {
                found = true;
                return true;
            });
            return found;
        }

        public string[] Search(string s, KWSearchOptions options)
        {
            var result = new List<string>();
            SearchWithAction(s, options, (kw) =>
            {
                result.Add(kw);
                return false;
            });
            return result.ToArray();
        }

        public int[] SearchIndices(string s, KWSearchOptions options)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string GetPattern(string pattern, KWSearchOptions options)
        {
            return options.HasFlag(KWSearchOptions.WholeWord) ? $"\b{pattern}\b" : pattern;
        }

        private void SearchWithAction(string s, KWSearchOptions options, Func<string, bool> processResult)
        {
            foreach (var kw in _kwList)
            {
                if (Regex.IsMatch(s, GetPattern(kw, options),
                    options.HasFlag(KWSearchOptions.IgnoreCase) ? RegexOptions.IgnoreCase : RegexOptions.None))
                {
                    if (processResult(kw))
                        return;
                }
            }
        }
    }
}
