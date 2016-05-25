using HCMUT.EMRCorefResol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    class CorefAnnotator
    {
        public event Action<CorefAnnotator, AnnotationOperationCompletedEventArgs> OperationCompleted;

        private readonly List<CorefChain> _editingChains;

        public IReadOnlyList<CorefChain> EditingChains
        {
            get { return _editingChains.AsReadOnly(); }
        }

        public CorefAnnotator(List<CorefChain> editingChains)
        {
            _editingChains = editingChains;
        }

        public Task<int> MergeChainAsync(IEnumerable<Concept> newConcepts)
        {
            return Task.Run(() =>
            {
                CorefChain containedChain = null;
                int chainIndex = -1;

                foreach (var c in newConcepts)
                {
                    containedChain = _editingChains.FindChainContains(c, out chainIndex);
                    if (containedChain != null)
                        break;
                }

                if (containedChain != null)
                {
                    var newChain = new List<Concept>(containedChain);
                    newChain.AddRange(newConcepts);
                    _editingChains[chainIndex] = new CorefChain(newChain, containedChain.Type);
                }

                return chainIndex;
            }).ContinueWith((Task<int> t) =>
            {
                var result = t.Result >= 0 ? AnnotationOperationResult.Changed : AnnotationOperationResult.UnChanged;
                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(result));
                return t.Result;
            });
        }

        public Task<ConceptType> CreateChainAsync(IEnumerable<Concept> newConcepts)
        {
            return Task.Run(() =>
            {
                var chainType = ConceptType.None;
                foreach (var c in newConcepts)
                {
                    if (chainType == ConceptType.None)
                    {
                        chainType = c.Type;
                    }
                    else if (chainType != c.Type)
                    {
                        if (chainType == ConceptType.Pronoun)
                        {
                            chainType = c.Type;
                        }
                        else
                        {
                            chainType = ConceptType.None;
                            break;
                        }
                    }
                }

                if (chainType != ConceptType.None && chainType != ConceptType.Pronoun)
                {
                    var newChain = new List<Concept>(newConcepts);
                    _editingChains.Add(new CorefChain(newChain, chainType));
                }

                return chainType;
            }).ContinueWith((Task<ConceptType> t) =>
            {
                var result = (t.Result == ConceptType.None || t.Result == ConceptType.Pronoun) ?
                    AnnotationOperationResult.UnChanged :
                    AnnotationOperationResult.Changed;
                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(result));
                return t.Result;
            });
        }

        public Task<bool> RemoveConceptsAsync(IEnumerable<Concept> oldConcepts)
        {
            return Task.Run(() =>
            {
                int index = -1;
                var removed = false;
                foreach (var c in oldConcepts)
                {
                    var oldChain = _editingChains.FindChainContains(c, out index);
                    if (oldChain != null)
                    {
                        if (oldChain.Count <= 2)
                        {
                            _editingChains.Remove(oldChain);
                        }
                        else
                        {
                            var newChain = new List<Concept>(oldChain);
                            newChain.Remove(c);
                            _editingChains[index] = new CorefChain(newChain, oldChain.Type);
                        }
                        removed = true;
                    }
                }
                return removed;
            }).ContinueWith((Task<bool> t) =>
            {
                var result = t.Result ? AnnotationOperationResult.Changed : AnnotationOperationResult.UnChanged;
                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(result));
                return t.Result;
            });
        }

        public bool IsIndexValid(int chainIndex)
        {
            return chainIndex >= 0 && chainIndex < _editingChains.Count;
        }

        public Task<bool> ChangeChainTypeAsync(int chainIndex, ConceptType newType)
        {
            return Task.Run(() =>
            {
                if (IsIndexValid(chainIndex))
                {
                    var chain = _editingChains[chainIndex];
                    if (chain.Type != newType)
                    {
                        var newChain = new CorefChain(chain, newType);
                        _editingChains[chainIndex] = newChain;
                        return true;
                    }
                }
                return false;
            }).ContinueWith((Task<bool> t) =>
            {
                var r = t.Result ? AnnotationOperationResult.Changed : AnnotationOperationResult.UnChanged;
                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(r));
                return t.Result;
            });
        }

        private void RaiseOperationCompleted(AnnotationOperationCompletedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                OperationCompleted?.Invoke(this, e);
            }));
        }
    }
}
