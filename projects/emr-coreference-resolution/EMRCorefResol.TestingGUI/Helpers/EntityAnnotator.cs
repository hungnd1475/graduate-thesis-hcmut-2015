using HCMUT.EMRCorefResol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    class EntityAnnotator
    {
        public event Action<EntityAnnotator, AnnotationOperationCompletedEventArgs> OperationCompleted;
        public event Action<CorefAnnotator, AnnotationOperationCompletedEventArgs> CorefOperationCompleted
        {
            add { _corefAnnotator.OperationCompleted += value; }
            remove { _corefAnnotator.OperationCompleted -= value; }
        }

        private readonly List<Concept> _editingConcepts;
        private readonly CorefAnnotator _corefAnnotator = null;

        public IReadOnlyList<Concept> EditingConcepts
        {
            get { return _editingConcepts.AsReadOnly(); }
        }

        public IReadOnlyList<CorefChain> EditingChains
        {
            get { return _corefAnnotator.EditingChains; }
        }

        public EntityAnnotator(List<Concept> editingConcepts,
            List<CorefChain> editingChains)
        {
            _editingConcepts = editingConcepts;
            _corefAnnotator = new CorefAnnotator(editingChains);
        }

        public void AddConcept(Concept newConcept)
        {
            var result = AnnotationOperationResult.UnChanged;
            Exception exception = null;
            try
            { 
                if (newConcept != null && !_editingConcepts.Contains(newConcept))
                {
                    _editingConcepts.Add(newConcept);
                    result = AnnotationOperationResult.Changed;
                }
            }
            catch (Exception ex)
            {
                result = AnnotationOperationResult.Faulted;
                exception = ex;
            }
            finally
            {
                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(result, "", exception));
            }
        }

        public Task<bool> RemoveConceptsAsync(IEnumerable<Concept> oldConcepts)
        {
            _corefAnnotator.RemoveConceptsAsync(oldConcepts);
            return Task.Run(() =>
            {
                var removed = false;
                foreach (var c in oldConcepts)
                {
                    if (_editingConcepts.Remove(c))
                    {
                        removed = true;
                    }
                }
                return removed;
            }).ContinueWith((Task<bool> t) =>
            {
                var result = AnnotationOperationResult.UnChanged;
                Exception exception = null;

                if (t.IsFaulted)
                {
                    result = AnnotationOperationResult.Faulted;
                    exception = t.Exception;
                }
                else if (t.Result)
                {
                    result = AnnotationOperationResult.Changed;
                }

                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(result, "", exception));
                return t.Result;
            });
        }

        public Task<bool> ChangeConceptType(Concept concept, ConceptType newType)
        {
            return Task.Run(() =>
            {
                if (concept.Type != newType)
                {
                    var index = _editingConcepts.IndexOf(concept);
                    if (index >= 0)
                    {
                        var newConcept = concept.Clone(newType);
                        _editingConcepts[index] = newConcept;
                        return true;
                    }
                }

                return false;
            }).ContinueWith((Task<bool> t) =>
            {
                if (t.Result)
                {
                    _corefAnnotator.RemoveConceptsAsync(new Concept[] { concept });
                }
                var result = t.Result ? AnnotationOperationResult.Changed : AnnotationOperationResult.UnChanged;
                RaiseOperationCompleted(new AnnotationOperationCompletedEventArgs(result));
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
