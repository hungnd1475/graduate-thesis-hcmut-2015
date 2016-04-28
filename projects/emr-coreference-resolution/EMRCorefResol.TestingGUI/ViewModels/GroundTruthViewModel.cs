using HCMUT.EMRCorefResol;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroundTruthViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private ObservableCollection<CorefChain> _editingChains;
        private bool _removingConcepts = false;

        private IEnumerable<CorefChain> _groundTruth;
        private IEnumerable<CorefChain> GroundTruth
        {
            get { return _groundTruth; }
            set
            {
                if (SetProperty(ref _groundTruth, value) && value != null)
                {
                    GTText = value.ToJointString();
                }
            }
        }

        private string _gtText;
        public string GTText
        {
            get { return _gtText; }
            set { SetProperty(ref _gtText, value); }
        }

        private IReadOnlyList<Concept> _focusedConcepts;
        public IReadOnlyList<Concept> FocusedConcepts
        {
            get { return _focusedConcepts; }
            set
            {
                if (SetProperty(ref _focusedConcepts, value))
                {
                    _eventAggregator.GetEvent<SelectedConceptChangedEvent>().Publish(null);
                    RemoveConceptsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand SelectConceptCommand { get; }
        public DelegateCommand RemoveConceptsCommand { get; }

        [ImportingConstructor]
        public GroundTruthViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            SelectConceptCommand = new DelegateCommand(SelectConcepts);
            RemoveConceptsCommand = new DelegateCommand(RemoveConcepts, () => _editingChains != null && _focusedConcepts.Count > 0);

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(EMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<CorefAnnotationBegunEvent>().Subscribe(OnCorefAnnotationBegun, ThreadOption.UIThread);
            eventAggregator.GetEvent<CorefAnnotationEndedEvent>().Subscribe(OnCorefAnnotationEnded, ThreadOption.UIThread);

            _regionManager = regionManager;
        }

        private async void RemoveConcepts()
        {
            _removingConcepts = true;

            await Task.Run(() =>
            {
                int index = -1;
                foreach (var c in _focusedConcepts)
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
                    }
                }
            });

            _removingConcepts = false;
            GTText = await _editingChains.ToJointStringAsync();
        }

        private void OnCorefAnnotationEnded(CorefChainCollection resultChains)
        {
            _editingChains.CollectionChanged -= EditingChains_CollectionChanged;
            _editingChains = null;

            GroundTruth = resultChains;
            RemoveConceptsCommand.RaiseCanExecuteChanged();
        }

        private void OnCorefAnnotationBegun(ObservableCollection<CorefChain> editingChains)
        {
            editingChains.CollectionChanged += EditingChains_CollectionChanged;

            GroundTruth = _editingChains = editingChains;            
            RemoveConceptsCommand.RaiseCanExecuteChanged();
        }

        private async void EditingChains_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_removingConcepts)
            {
                GTText = await _editingChains.ToJointStringAsync();
            }
        }

        private void SelectConcepts()
        {        
            Concept c = null;
            if (_focusedConcepts != null)
            {
                c = _focusedConcepts.Count > 0 ? _focusedConcepts[_focusedConcepts.Count - 1] : null;
            }
            _eventAggregator.GetEvent<SelectedConceptChangedEvent>().Publish(c);
        }

        private void EMRChanged(EMRChangedEventArgs e)
        {
            GroundTruth = e?.GroundTruth;
        }
    }
}
