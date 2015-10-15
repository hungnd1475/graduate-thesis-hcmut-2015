using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class MedicationInfoCollection : IIndexedEnumerable<MedicationInfo>
    {
        private readonly List<MedicationInfo> _infos = new List<MedicationInfo>();

        public MedicationInfo this[int index]
        {
            get { return _infos[index]; }
        }

        public int Count
        {
            get { return _infos.Count; }
        }

        public MedicationInfoCollection(string infosFile, IDataReader dataReader)
        {
            var fs = new FileStream(infosFile, FileMode.Open);
            var sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if(line==null || line.Length <= 0)
                {
                    continue;
                }

                var i = dataReader.ReadMedInfoLine(line);
                if (i != null)
                {
                    _infos.Add(i);
                }
            }
        }

        public int IndexOf(MedicationInfo info)
        {
            return _infos.IndexOf(info);
        }

        public IEnumerator<MedicationInfo> GetEnumerator()
        {
            return _infos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
