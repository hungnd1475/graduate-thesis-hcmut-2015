using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents a collection of <see cref="CorefChain"/>(s) resolved from an EMR.
    /// </summary>
    public class CorefChainCollection : IIndexedEnumerable<CorefChain>
    {
        private readonly IList<CorefChain> _chains;
        private CorefChain _patientChain;
        private readonly object _lockObj = new object();
        private bool _searchedForPt = false;

        /// <summary>
        /// Gets the total number of chains in the collection.
        /// </summary>
        public int Count
        {
            get { return _chains.Count; }
        }

        public CorefChain this[int index]
        {
            get { return _chains[index]; }
        }

        /// <summary>
        /// Initializes a <see cref="CorefChainCollection"/> instance from a chains file.
        /// </summary>
        /// <param name="chainsFile">The path to the chains file.</param>
        /// <param name="dataReader">The reader that can read the concepts in the chains file.</param>
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
                var chain = new CorefChain(concepts, type);
                _chains.Add(chain);
            }
            sr.Close();
        }

        /// <summary>
        /// Initializes a <see cref="CorefChainCollection"/> instance from a collection of <see cref="CorefChain"/> instances.
        /// </summary>
        /// <param name="chains">The collection of <see cref="CorefChain"/> instances.</param>
        public CorefChainCollection(ICollection<CorefChain> chains)
        {
            _chains = new List<CorefChain>(chains);
        }

        /// <summary>
        /// Initializes a <see cref="CorefChainCollection"/> instance from a list of <see cref="CorefChain"/> instances.
        /// </summary>
        /// <param name="chains">The list of <see cref="CorefChain"/> instances</param>
        public CorefChainCollection(IList<CorefChain> chains)
        {
            _chains = chains;
        }

        /// <summary>
        /// Gets the <see cref="CorefChain"/> about the patient of the EMR.
        /// </summary>
        /// <returns></returns>
        public CorefChain GetPatientChain(IKeywordDictionary patientKW, IKeywordDictionary relativeKW)
        {
            lock (_lockObj)
            {
                if (!_searchedForPt)
                {
                    var t = GetPatientChainByKW(patientKW);
                    _patientChain = t != null ? t : GetPatientChainByLength(relativeKW);
                    _searchedForPt = true;
                }
            }
            return _patientChain;
        }

        private CorefChain GetPatientChainByKW(IKeywordDictionary patientKW)
        {
            return _chains.FirstOrDefault(ch =>
                ch.Type == ConceptType.Person &&
                ch.Any(c => patientKW.Match(c.Lexicon, KWSearchOptions.IgnoreCase | KWSearchOptions.WholeWord))
            );
        }

        private CorefChain GetPatientChainByLength(IKeywordDictionary relativeKW)
        {
            CorefChain r = null;
            foreach (var ch in _chains)
            {
                if (ch.Type == ConceptType.Person && !ch.Any(c => relativeKW.Match(c.Lexicon, KWSearchOptions.WholeWord)))
                {
                    if (r == null || r.Count < ch.Count)
                        r = ch;
                }
            }
            return r;
        }

        /// <summary>
        /// Finds the <see cref="CorefChain"/> that contains a specified <see cref="Concept"/>.
        /// </summary>
        /// <param name="concept">The <see cref="Concept"/> instance to perform the finding operation.</param>
        /// <returns></returns>
        public CorefChain FindChainContains(Concept concept)
        {
            foreach (var c in _chains)
            {
                if (c.Contains(concept))
                    return c;
            }
            return null;
        }

        /// <summary>
        /// Checks if a concept pair is coreferent.
        /// </summary>
        /// <param name="pair">The concept pair.</param>
        /// <returns></returns>
        public bool IsCoref(IConceptPair pair)
        {
            return IsCoref(pair.Antecedent, pair.Anaphora);
        }

        /// <summary>
        /// Checks if two concepts is coreferent with each other.
        /// </summary>
        /// <param name="first">The fist concept.</param>
        /// <param name="second">The second concept.</param>
        /// <returns></returns>
        public bool IsCoref(Concept first, Concept second)
        {
            CorefChain chain;
            return IsCoref(first, second, out chain);
        }

        /// <summary>
        /// Checks if a concept pair is coreferent
        /// and returns an additional <see cref="CorefChain"/> instance contains the pair if coreferent.
        /// </summary>
        /// <param name="pair">The concept pair.</param>
        /// <param name="chain">The chain contains the pair. Null if the pair is not reference.</param>
        /// <returns></returns>
        public bool IsCoref(IConceptPair pair, out CorefChain chain)
        {
            return IsCoref(pair.Antecedent, pair.Anaphora, out chain);
        }

        /// <summary>
        /// Checks if two concepts is coreferent with each other
        /// and returns an additional <see cref="CorefChain"/> instance contains the two concepts if they are coreferent.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="chain"></param>
        /// <returns></returns>
        public bool IsCoref(Concept first, Concept second, out CorefChain chain)
        {
            chain = FindChainContains(first);
            return chain != null ? chain.Contains(second) : false;
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
