using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    using Service;
    using System.IO;

    public class UmlsDataDictionary
    {
        private readonly Dictionary<string, UMLSData> _infos = new Dictionary<string, UMLSData>();

        public UMLSData Get(string key)
        {
            if (_infos.ContainsKey(key))
            {
                return _infos[key];
            } else
            {
                return null;
            }
        }

        public int Count
        {
            get { return _infos.Count; }
        }

        public UmlsDataDictionary(string infosFile, IDataReader dataReader)
        {
            var fs = new FileStream(infosFile, FileMode.Open);
            var sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var i = dataReader.ReadUmlsFile(line);
                if (i != null)
                {
                    _infos.Add(i.Item1, i.Item2);
                }
            }

            sr.Close();
            fs.Close();
        }
    }
}
