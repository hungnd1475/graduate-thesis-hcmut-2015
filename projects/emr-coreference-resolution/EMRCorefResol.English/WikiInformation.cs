using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using IO;
    class WikiInformation
    {
        private static Dictionary<string, WikiDataDictionary> _dictionary;

        static WikiInformation()
        {
            _dictionary = new Dictionary<string, WikiDataDictionary>();
        }

        public static WikiDataDictionary GetWikiInfo(string emrPath)
        {
            if (!File.Exists(emrPath))
            {
                return null;
            }

            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(emrPath))
                {
                    return _dictionary[emrPath];
                }

                return GetWikiFile(emrPath);
            }
        }

        private static WikiDataDictionary GetWikiFile(string emrPath)
        {
            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var dataReader = new I2B2DataReader();
            var wikiPath = Path.Combine(new string[] { rootPath, "wiki", fileName });
            if (!File.Exists(wikiPath))
            {
                return null;
            }


            var wikiDictionary = new WikiDataDictionary(wikiPath, dataReader);
            _dictionary.Add(emrPath, wikiDictionary);

            return wikiDictionary;
        }
    }
}
