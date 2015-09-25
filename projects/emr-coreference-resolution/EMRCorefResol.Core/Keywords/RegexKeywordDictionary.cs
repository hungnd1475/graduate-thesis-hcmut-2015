using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HCMUT.EMRCorefResol.Keywords
{
    public class RegexKeywordDictionary : IKeywordDictionary
    {
        private IEnumerable<string> _kwList;

        public RegexKeywordDictionary(params string[] keywords)
        {
            _kwList = keywords;
        }

        public bool Match(string s, bool wholeWord)
        {
            foreach (var kw in _kwList)
            {
                if (Regex.IsMatch(s, GetPattern(kw, wholeWord), RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }

        private string GetPattern(string pattern, bool wholeWord)
        {
            return wholeWord ? $"\b{pattern}\b" : pattern;
        }
    }
}
