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
        private readonly SortedList<ConceptPosition, Concept> _concepts
            = new SortedList<ConceptPosition, Concept>();

        public Concept this[int index]
        {
            get { return _concepts.Values[index]; }
        }

        public Concept this[ConceptPosition begin]
        {
            get { return _concepts[begin]; }
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
                    _concepts.Add(c.Begin, c);
                }
            }
            sr.Close();
        }

        public int IndexOf(Concept concept)
        {
            return _concepts.IndexOfValue(concept);
        }

        public IEnumerator<Concept> GetEnumerator()
        {
            return _concepts.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
