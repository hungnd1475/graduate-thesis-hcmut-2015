using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    using Service;
    using System.IO;

    public class WikiDataDictionary
    {
        private readonly Dictionary<string, WikiData> _infos = new Dictionary<string, WikiData>();

        public WikiData Get(string key)
        {
            if (_infos.ContainsKey(key))
            {
                return _infos[key];
            }
            else
            {
                return null;
            }
        }

        public int Count
        {
            get { return _infos.Count; }
        }

        public WikiDataDictionary(string infosFile, IDataReader dataReader)
        {
            var fs = new FileStream(infosFile, FileMode.Open);
            var sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == null || line.Length <= 0)
                {
                    continue;
                }

                var i = dataReader.ReadWikiFile(line);
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
