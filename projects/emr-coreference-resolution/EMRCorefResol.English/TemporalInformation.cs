using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using IO;
    class TemporalInformation
    {
        private static Dictionary<string, string> _dictionary;

        public static TemporalDataDictionary GetTemporalFile(string emrPath)
        {
            //Console.WriteLine("Loading medication information...");

            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var dataReader = new I2B2DataReader();
            var temporalPath = Path.Combine(new string[] { rootPath, "temporal", fileName });
            if (!File.Exists(temporalPath))
            {
                return null;
            }

            var tempCollection = new TemporalDataDictionary(temporalPath, dataReader);

            return tempCollection;
        }
    }
}
