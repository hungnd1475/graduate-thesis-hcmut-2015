using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class ConceptCollection : IIndexedEnumerable<Concept>
    {
        private readonly Dictionary<ConceptPosition, Concept> _concepts = 
            new Dictionary<ConceptPosition, Concept>();

        private List<ConceptPosition> _positions = 
            new List<ConceptPosition>();

        public Concept this[int index]
        {
            get { return _concepts[_positions[index]]; }
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
                    _positions.Add(c.Begin);
                }
            }
            sr.Close();
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
