using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.IO;
using System.IO;
using System.Text.RegularExpressions;

namespace HCMUT.EMRCorefResol
{
    public class WikiDataDictionary : WorldKnowledgeDictionary<string, WikiData>
    {
        public static readonly IWorldKnowledgeReader<string, WikiData> Reader
            = new WikiDataReader();

        public static WikiDataDictionary LoadFromEMRPath(string emrPath, string wikiDirName)
        {
            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var wikiPath = Path.Combine(new string[] { rootPath, wikiDirName, fileName });
            return File.Exists(wikiPath) ? new WikiDataDictionary(wikiPath) : null;
        }

        private readonly Dictionary<string, WikiData> _wikiData 
            = new Dictionary<string, WikiData>();

        public WikiDataDictionary(string wikiFile)
            : base(wikiFile, Reader)
        { }

        protected override void Add(string key, WikiData value)
        {
            _wikiData.Add(key, value);
        }

        public override WikiData Get(string key)
        {
            return _wikiData.ContainsKey(key) ? _wikiData[key] : null;
        }

        class WikiDataReader : IWorldKnowledgeReader<string, WikiData>
        {
            private static Regex WikiDataPattern = 
                new Regex("rawTerm=\"(.*?)\"\\|\\|term=\"(.*?)\"\\|\\|title=\"(.*?)\"\\|\\|links=\\[(.*?)\\]\\|\\|bolds=\\[(.*?)\\]");

            public bool Read(string line, out string key, out WikiData value)
            {
                var match = WikiDataPattern.Match(line);
                if (match.Success)
                {
                    key = match.Groups[1].Value;
                    var term = match.Groups[2].Value;
                    var title = match.Groups[3].Value;
                    var links = match.Groups[4].Value.Split('|');
                    var bolds = match.Groups[5].Value.Split('|');
                    value = new WikiData(term, title, links, bolds);

                    return true;
                }
                else
                {
                    key = null;
                    value = null;
                    return false;
                }
            }
        }
    }
}
