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
    public class UMLSDataDictionary : WorldKnowledgeDictionary<string, UMLSData>
    {
        public static readonly IWorldKnowledgeReader<string, UMLSData> Reader = new UMLSDataReader();

        public static UMLSDataDictionary LoadFromEMRPath(string emrPath, string umlsDirName)
        {
            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var umlsPath = Path.Combine(new string[] { rootPath, umlsDirName, fileName });
            return File.Exists(umlsPath) ? new UMLSDataDictionary(umlsPath) : null;
        }

        private readonly Dictionary<string, UMLSData> _umlsData 
            = new Dictionary<string, UMLSData>();

        public UMLSDataDictionary(string umlsFile)
            : base(umlsFile, Reader)
        { }

        public override UMLSData Get(string key)
        {
            UMLSData result;
            return _umlsData.TryGetValue(key, out result) ? result : null;
        }

        protected override void Add(string key, UMLSData value)
        {
            _umlsData.Add(key, value);
        }

        class UMLSDataReader : IWorldKnowledgeReader<string, UMLSData>
        {
            private static Regex UmlsDataPattern = 
                new Regex("rawTerm=\"(.*?)\"\\|\\|cui=\"(.*?)\"\\|\\|concept=\"(.*?)\"\\|\\|prefer=\"(.*?)\"\\|\\|semantic=\\[(.*?)\\]\\|\\|confidence=\"(.*?)\"");

            public bool Read(string line, out string key, out UMLSData value)
            {
                var match = UmlsDataPattern.Match(line);
                if (match.Success)
                {
                    key = match.Groups[1].Value;
                    var cui = match.Groups[2].Value;
                    var concept = match.Groups[3].Value;
                    var prefer = match.Groups[4].Value;
                    var semantic = match.Groups[5].Value.Split('|');
                    var confidence = int.Parse(match.Groups[6].Value);
                    value = new UMLSData(cui, concept, prefer, semantic, confidence);
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
