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
        public static TemporalDataDictionary GetTemporalFile(string emrPath)
        {
            //Console.WriteLine("Loading medication information...");

            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var temporalPath = Path.Combine(new string[] { rootPath, "new_temporal", fileName + ".ann" });
            if (!File.Exists(temporalPath))
            {
                return null;
            }

            var tempCollection = new TemporalDataDictionary(temporalPath);

            return tempCollection;
        }
    }
}
