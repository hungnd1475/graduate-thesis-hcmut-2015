using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class TemporalDataDictionary
    {
        private readonly Dictionary<string, string> _temporals = new Dictionary<string, string>();

        public string Get(string key)
        {
            if (_temporals.ContainsKey(key))
            {
                return _temporals[key];
            }
            else
            {
                return null;
            }
        }

        public int Count
        {
            get { return _temporals.Count; }
        }

        public TemporalDataDictionary(string infosFile, IDataReader dataReader)
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

                var i = dataReader.ReadTemporalFile(line);
                if (i != null)
                {
                    _temporals.Add(i.Item1, i.Item2);
                }
            }

            sr.Close();
            fs.Close();
        }
    }
}
