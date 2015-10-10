using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class EMRSectionCollection : IIndexedEnumerable<EMRSection>
    {
        private readonly List<EMRSection> _sections = new List<EMRSection>();

        public EMRSection this[int index]
        {
            get { return _sections[index]; }
        }

        public int Count
        {
            get { return _sections.Count; }
        }

        public EMRSectionCollection(string emrContent, IDataReader dataReader)
        {
            var sections = dataReader.ReadSection(emrContent);
            if(sections != null)
            {
                _sections = sections;
            }

            _sections.Sort(SectionComparison);
        }

        private static int SectionComparison(EMRSection s1, EMRSection s2)
        {
            var compareBegin = s1.Begin.CompareTo(s2.Begin);
            return compareBegin != 0 ? compareBegin : s1.End.CompareTo(s2.End);
        }

        public int IndexOf(EMRSection section)
        {
            return _sections.IndexOf(section);
        }

        public IEnumerator<EMRSection> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
