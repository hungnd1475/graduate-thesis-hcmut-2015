using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    using IO;
    class MedicationInformation
    {
        private static Dictionary<string, MedicationInfoCollection> _meds;

        static MedicationInformation()
        {
            _meds = new Dictionary<string,MedicationInfoCollection>();
        }

        public static MedicationInfoCollection GetMedicationInfo(string emrPath)
        {
            if (!File.Exists(emrPath))
            {
                return null;
            }

            lock (_meds)
            {
                if (_meds.ContainsKey(emrPath))
                {
                    return _meds[emrPath];
                }

                return GetMedicationFile(emrPath);
            }
        }

        public static MedicationInfoCollection GetMedicationFile(string emrPath)
        {
            //Console.WriteLine("Loading medication information...");

            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var dataReader = new I2B2DataReader();
            var medPath = Path.Combine(new string[] { rootPath, "medications", fileName });
            if (!File.Exists(medPath))
            {
                return null;
            }

            var medCollection = new MedicationInfoCollection(medPath, dataReader);
            //_meds.Add(emrPath, medCollection);

            return medCollection;
        }
    }
}
