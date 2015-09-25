using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public interface IKeywordDictionary : IEnumerable<string>
    {
        bool Match(string s, KWSearchOptions options);
        string[] Search(string s, KWSearchOptions options);
    }

    [Flags]
    public enum KWSearchOptions
    {
        None = 0,
        IgnoreCase = 1,
        WholeWord = 2,
    }
}
