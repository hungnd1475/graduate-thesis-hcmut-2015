using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.IO;
using System.Text.RegularExpressions;

namespace HCMUT.EMRCorefResol
{
    public class MedDataDictionary : WorldKnowledgeDictionary<MedKey, MedData>
    {
        public static readonly IWorldKnowledgeReader<MedKey, MedData> Reader =
            new MedDataReader();

        public static MedDataDictionary LoadFromEMRPath(string emrPath, string medDirName)
        {
            var fileInfo = new FileInfo(emrPath);
            var rootPath = fileInfo.Directory.Parent.FullName;
            var fileName = fileInfo.Name;

            var medPath = Path.Combine(new string[] { rootPath, "medications", fileName });
            return File.Exists(medPath) ? new MedDataDictionary(medPath) : null;
        }

        private readonly List<MedData> _medData = new List<MedData>();

        public MedDataDictionary(string medFile)
            : base(medFile, Reader)
        { }

        protected override void Add(MedKey key, MedData value)
        {
            _medData.Add(value);
        }

        public override MedData Get(MedKey key)
        {
            var emr = key.EMR;
            var c = key.Concept;
            var line = emr.GetLine(c);

            foreach (MedData med in _medData)
            {
                if (string.Equals(line.Replace("\r", ""), med.Line))
                {
                    if (c.Lexicon.ToLower().Contains(med.Drug.ToLower()) ||
                        med.Drug.ToLower().Contains(c.Lexicon.ToLower()))
                    {
                        return med;
                    }
                }
            }

            return null;
        }

        class MedDataReader : IWorldKnowledgeReader<MedKey, MedData>
        {
            public bool Read(string line, out MedKey key, out MedData value)
            {
                var info_arr = line.Split('|');

                var line_info = info_arr[0].Split('\t');
                int line_index = 0;
                int.TryParse(line_info[0], out line_index);
                var line_value = line_info[1];

                var regex = @"\[(.*?)\]";
                var drug = Regex.Replace(info_arr[1], regex, "");
                var form = Regex.Replace(info_arr[3], regex, "");
                var strength = Regex.Replace(info_arr[4], regex, "");
                var dose = Regex.Replace(info_arr[5], regex, "");
                var route = Regex.Replace(info_arr[6], regex, "");
                var freq = Regex.Replace(info_arr[7], regex, "");
                var duration = Regex.Replace(info_arr[8], regex, "");
                var neccessity = Regex.Replace(info_arr[9], regex, "");

                key = new MedKey();
                value = new MedData(line_index, line_value, drug, form, 
                    strength, dose, route, freq, duration, neccessity);

                return true;
            }
        }
    }
}
