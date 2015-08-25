using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol
{
    public class CorefChainCollection : IEnumerable<CorefChain>
    {
        private readonly ICollection<CorefChain> _chains;

        public int Count
        {
            get { return _chains.Count; }
        }

        public CorefChainCollection(string chainsFile, IDataReader dataReader)
        {
            var fs = new FileStream(chainsFile, FileMode.Open);
            var sr = new StreamReader(fs);
            _chains = new Collection<CorefChain>();

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var concepts = dataReader.ReadMultiple(line).OrderBy(c => c.Begin);
                var type = dataReader.ReadType(line);
                var chain = new CorefChain(concepts, concepts.First(), type);
                _chains.Add(chain);
            }
            sr.Close();
        }

        public CorefChainCollection(ICollection<CorefChain> chains)
        {
            _chains = chains;
        }

        public CorefChainCollection(IList<CorefChain> chains)
        {
            _chains = new Collection<CorefChain>(chains);
        }

        public CorefChain GetPatientChain()
        {
            CorefChain result = null;
            foreach (var c in _chains)
            {
                if (c.Type == ConceptType.Person)
                {
                    if (result == null || c.Count > result.Count)
                    {
                        result = c;
                    }
                }
            }
            return result;
        }

        public CorefChain FindChainContains(Concept concept)
        {
            foreach (var c in _chains)
            {
                if (c.Contains(concept))
                    return c;
            }
            return null;
        }

        #region Enumerator

        public IEnumerator<CorefChain> GetEnumerator()
        {
            return _chains.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
