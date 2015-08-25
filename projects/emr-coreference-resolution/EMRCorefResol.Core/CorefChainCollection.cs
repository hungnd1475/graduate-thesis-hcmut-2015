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
    public class CorefChainCollection : ICollection<CorefChain>, IEnumerable<CorefChain>
    {
        private readonly Collection<CorefChain> _chains = new Collection<CorefChain>();

        public CorefChain this[int index]
        {
            get { return _chains[index]; }
        }

        public int Count
        {
            get { return _chains.Count; }
        }

        public bool IsReadOnly { get; private set; } = false;

        public CorefChainCollection(string chainsFile, IDataReader dataReader)
        {
            var fs = new FileStream(chainsFile, FileMode.Open);
            var sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var concepts = dataReader.ReadMultiple(line).OrderBy(c => c.Begin);
                var type = dataReader.ReadType(line);
                var chain = new CorefChain(concepts.First(), type);
                foreach (var c in concepts)
                {
                    chain.Add(c);
                }
                _chains.Add(chain);
            }
            sr.Close();
        }

        public void Seal()
        {
            IsReadOnly = true;
        }

        private void CheckOperation()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("The collection is currently read only.");
            }
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

        #region Collection

        public void Add(CorefChain chain)
        {
            CheckOperation();
            _chains.Add(chain);
        }

        public void Clear()
        {
            CheckOperation();
            _chains.Clear();
        }

        public bool Contains(CorefChain chain)
        {
            return _chains.Contains(chain);
        }

        public void CopyTo(CorefChain[] array, int arrayIndex)
        {
            _chains.CopyTo(array, arrayIndex);
        }

        public bool Remove(CorefChain item)
        {
            CheckOperation();
            return _chains.Remove(item);
        }

        #endregion
    }
}
