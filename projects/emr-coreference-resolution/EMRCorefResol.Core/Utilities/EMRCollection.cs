using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol
{
    public class EMRCollection
    {
        private readonly string[] _emrPaths;
        private readonly string _conceptsDir, _chainsDir;

        public int Count { get { return _emrPaths.Length; } }

        public EMRCollection(string emrDir, string conceptsDir, string chainsDir)
        {
            _emrPaths = Directory.GetFiles(emrDir);
            _conceptsDir = conceptsDir;
            _chainsDir = chainsDir;
        }

        public string GetEMRPath(int index)
        {
            return _emrPaths[index];
        }

        public string GetConceptsPath(int index)
        {
            var emrPath = _emrPaths[index];
            var emrFileName = Path.GetFileName(emrPath);
            return Path.Combine(_conceptsDir, emrFileName + ".con");
        }

        public string GetChainsPath(int index)
        {
            var emrPath = _emrPaths[index];
            var emrFileName = Path.GetFileName(emrPath);
            return Path.Combine(_chainsDir, emrFileName + ".chains");
        }
    }
}
