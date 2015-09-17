using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents a collection of concepts appear in an EMR
    /// and guaranteed to be sorted by position in which they appears.
    /// </summary>
    public class ConceptCollection : IIndexedEnumerable<Concept>
    {
        private readonly List<Concept> _concepts = new List<Concept>();

        public Concept this[int index]
        {
            get { return _concepts[index]; }
        }

        public int Count
        {
            get { return _concepts.Count; }
        }

        public ConceptCollection(string conceptsFile, IDataReader dataReader)
        {
            var fs = new FileStream(conceptsFile, FileMode.Open);
            var sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var c = dataReader.ReadSingle(line);
                if (c != null)
                {
                    _concepts.Add(c);
                }
            }

            _concepts.Sort(ConceptComparison);

            sr.Close();
        }

        private static int ConceptComparison(Concept c1, Concept c2)
        {
            var compareBegin = c1.Begin.CompareTo(c2.Begin);
            return compareBegin != 0 ? compareBegin : c1.End.CompareTo(c2.End);
        }

        public int IndexOf(Concept concept)
        {
            return _concepts.IndexOf(concept);
        }

        public IEnumerator<Concept> GetEnumerator()
        {
            return _concepts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
