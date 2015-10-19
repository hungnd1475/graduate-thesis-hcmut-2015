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
        private static Dictionary<string, Dictionary<string, MedicationInfoCollection>> _meds;

        static MedicationInformation()
        {
            _meds = new Dictionary<string, Dictionary<string, MedicationInfoCollection>>();
        }

        public static MedicationInfoCollection GetMedicationInfo(string rootPath, string emrName)
        {
            if (!Directory.Exists(rootPath))
            {
                return null;
            }

            var rootInfo = new DirectoryInfo(rootPath);

            lock (_meds)
            {
                if (_meds.ContainsKey(rootInfo.Name))
                {
                    return GetMedicationFile(rootInfo, emrName);
                }

                _meds[rootInfo.Name] = new Dictionary<string, MedicationInfoCollection>();
                return GetMedicationFile(rootInfo, emrName);
            }
        }

        private static DirectoryInfo GetSubFolder(DirectoryInfo root, string folderName)
        {
            var childrenFolder = root.GetDirectories();

            foreach(DirectoryInfo d in childrenFolder)
            {
                if(string.Equals(d.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return d;
                }
            }

            return null;
        }

        private static MedicationInfoCollection GetMedicationFile(DirectoryInfo datasetPath, string fileName)
        {
            var _dataset = _meds[datasetPath.Name];

            if (_dataset.ContainsKey(fileName))
            {
                return _dataset[fileName];
            }

            var dataReader = new I2B2DataReader();
            var medPath = Path.Combine(new string[] { datasetPath.FullName, "medications", fileName });
            if (!File.Exists(medPath))
            {
                return null;
            }


            var medCollection = new MedicationInfoCollection(medPath, dataReader);

            _meds[datasetPath.Name][fileName] = medCollection;

            return medCollection;
        }
    }
}
