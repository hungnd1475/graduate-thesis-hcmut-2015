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
        private readonly string _conceptsDir, _chainsDir, _medicationsDir, _emrDir;
        private readonly Random _rand = new Random();

        public int Count { get { return _emrPaths.Length; } }

        public bool HasGroundTruth { get; }

        public EMRCollection(string emrDir, string conceptsDir, string chainsDir, string medicationsDir)
        {
            _emrPaths = Directory.GetFiles(emrDir);
            _conceptsDir = conceptsDir;
            _chainsDir = chainsDir;
            _medicationsDir = medicationsDir;
            _emrDir = emrDir;

            HasGroundTruth = Directory.Exists(_chainsDir);
        }

        public EMRCollection(string dir)
            : this(Path.Combine(dir, "docs"),
                  Path.Combine(dir, "concepts"),
                  Path.Combine(dir, "chains"),
                  Path.Combine(dir, "medications"))
        { }

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

        public string GetMedicationsPath(int index)
        {
            var emrPath = _emrPaths[index];
            var emrFileName = Path.GetFileName(emrPath);
            return Path.Combine(_medicationsDir, emrFileName);
        }

        public void GetRandom(int size, out string[] emrPaths, out string[] conceptsPaths, out string[] chainsPaths)
        {
            if (size > Count)
            {
                emrPaths = new string[Count];
                conceptsPaths = new string[Count];
                chainsPaths = new string[Count];

                for (int i = 0; i < Count; i++)
                {
                    emrPaths[i] = GetEMRPath(i);
                    conceptsPaths[i] = GetConceptsPath(i);
                    chainsPaths[i] = GetChainsPath(i);
                }
            }
            else
            {
                emrPaths = new string[size];
                conceptsPaths = new string[size];
                chainsPaths = new string[size];

                var indices = new HashSet<int>();
                int k;

                for (int i = 0; i < size; i++)
                {
                    do
                    {
                        k = _rand.Next(0, Count - 1);
                    }
                    while (indices.Contains(k));

                    indices.Add(k);
                    emrPaths[i] = GetEMRPath(k);
                    conceptsPaths[i] = GetConceptsPath(k);
                    chainsPaths[i] = GetChainsPath(k);
                }
            }
        }

        public int IndexOf(string emrFileName)
        {
            return Array.IndexOf(_emrPaths, Path.Combine(_emrDir, emrFileName));
        }

        public static bool IsPathOk(string emrTopLevelDir)
        {
            return Directory.Exists(Path.Combine(emrTopLevelDir, "docs"))
                && Directory.Exists(Path.Combine(emrTopLevelDir, "concepts"));
        }
    }
}
