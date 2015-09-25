using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Keywords
{
    public class AhoCorasickKeywordDictionary
    {
        class TrieNode
        {
            public Dictionary<char, TrieNode> NextNodes { get; }
                = new Dictionary<char, TrieNode>();


        }
    }
}
