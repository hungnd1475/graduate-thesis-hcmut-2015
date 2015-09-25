using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Utilities
{
    public class AhoCorasickKeywordDictionary : IKeywordDictionary
    {
        private readonly TrieNode _root = new TrieNode();
        private readonly string[] _kwList;

        public AhoCorasickKeywordDictionary(params string[] keywords)
        {
            _kwList = keywords;            
            BuildTree();
            BuildACAutomaton();
        }

        public AhoCorasickKeywordDictionary(IEnumerable<string> keywords)
            : this(keywords.ToArray())
        { }

        private void BuildTree()
        {
            for (int i = 0; i < _kwList.Length; i++)
            {
                var kw = _kwList[i].ToLower();
                var curNode = _root;
                var sb = new StringBuilder();

                for (int j = 0; j < kw.Length; j++)
                {
                    var c = kw[j];
                    sb.Append(c);

                    if (curNode.NextNodes.ContainsKey(c))
                    {
                        curNode = curNode.NextNodes[c];
                    }
                    else
                    {
                        var node = new TrieNode();
                        node.ExactKWIndex = -1;
                        curNode.NextNodes.Add(c, node);
                        curNode = node;
                    }
                }

                curNode.ExactKWIndex = i;
                curNode.KWIndices.Add(i);
            }
        }

        private void BuildACAutomaton()
        {
            var q = new Queue<TrieNode>();
            foreach (var n in _root.NextNodes.Values)
            {
                q.Enqueue(n);
                n.FailNode = _root;
            }

            while (q.Count > 0)
            {
                var r = q.Dequeue();
                foreach (var t in r.NextNodes)
                {
                    var a = t.Key;
                    var u = t.Value;

                    q.Enqueue(u);
                    var v = r.FailNode;
                    while (v == _root && !v.NextNodes.ContainsKey(a))
                    {
                        v = v.FailNode;
                    }
                    u.FailNode = (v != null && v.NextNodes.ContainsKey(a)) ? v.NextNodes[a] : _root;
                    u.KWIndices.UnionWith(u.FailNode.KWIndices);
                }
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _kwList.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Match(string s, KWSearchOptions options)
        {
            var found = false;

            SearchWithAction(s, options, (i, node) =>
            {
                if (options.HasFlag(KWSearchOptions.WholeWord))
                {
                    if (i == s.Length - 1 || s[i + 1] == ' ')
                    {
                        foreach (var kwi in node.KWIndices)
                        {
                            var kw = _kwList[kwi];
                            var bIndex = i - kw.Length;
                            if (bIndex < 0 || s[bIndex] == ' ')
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    found = true;
                }
                return true;
            });

            return found;
        }

        public string[] Search(string s, KWSearchOptions options)
        {
            var outIndices = new HashSet<int>();

            SearchWithAction(s, options, (i, node) =>
            {
                if (options.HasFlag(KWSearchOptions.WholeWord))
                {
                    if (i == s.Length - 1 || s[i + 1] == ' ')
                    {
                        foreach (var kwi in node.KWIndices)
                        {
                            var kw = _kwList[kwi];
                            var bIndex = i - kw.Length;
                            if (bIndex < 0 || s[bIndex] == ' ')
                                outIndices.Add(kwi);
                        }
                    }
                }
                else
                {
                    outIndices.UnionWith(node.KWIndices);
                }
                return false;
            });

            var result = new string[outIndices.Count];
            var t = outIndices.GetEnumerator();
            for (int i = 0; t.MoveNext() && i < result.Length; i++)
            {
                result[i] = _kwList[t.Current];
            }
            return result;
        }

        private void SearchWithAction(string s, KWSearchOptions option, Func<int, TrieNode, bool> processResult)
        {
            var curNode = _root;
            if (option.HasFlag(KWSearchOptions.IgnoreCase))
            {
                s = s.ToLower();
            }

            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                while (curNode != null && !curNode.NextNodes.ContainsKey(c))
                {
                    curNode = curNode.FailNode;
                }

                if (curNode != null)
                {
                    curNode = curNode.NextNodes[c];
                    if (curNode.KWIndices.Count > 0)
                    {
                        if (processResult(i, curNode))
                            return;
                    }
                }
                else
                {
                    curNode = _root;
                }
            }
        }

        class TrieNode
        {
            public Dictionary<char, TrieNode> NextNodes { get; }
                = new Dictionary<char, TrieNode>();

            public HashSet<int> KWIndices { get; }
                = new HashSet<int>();

            public int ExactKWIndex { get; set; }

            public TrieNode FailNode { get; set; }
        }
    }
}
