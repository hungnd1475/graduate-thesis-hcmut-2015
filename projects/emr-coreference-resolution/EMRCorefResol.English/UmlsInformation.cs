using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using IO;
    using System.IO;

    class UmlsInformation
    {
        private static Dictionary<string, UmlsDataDictionary> _dictionary;

        static UmlsInformation()
        {
            _dictionary = new Dictionary<string, UmlsDataDictionary>();
        }

        public static UmlsDataDictionary GetUmlsInfo(string emrPath)
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

        public static UmlsDataDictionary GetWikiFile(string emrPath)
        {
            //Console.WriteLine("Loading umls data...");

            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var dataReader = new I2B2DataReader();
            var umlsPath = Path.Combine(new string[] { rootPath, "umls", fileName });
            if (!File.Exists(umlsPath))
            {
                return null;
            }


            var umlsDictionary = new UmlsDataDictionary(umlsPath, dataReader);
            //_dictionary.Add(emrPath, umlsDictionary);

            return umlsDictionary;
        }
    }
}
