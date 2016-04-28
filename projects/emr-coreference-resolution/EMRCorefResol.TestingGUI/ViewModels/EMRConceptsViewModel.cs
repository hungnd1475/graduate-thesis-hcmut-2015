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
    public class EMRConceptsViewModel : BindableBase
    {
        private static readonly string EMPTY_EMR_CONCEPTS = "<empty>";
        private static readonly ConceptType[] CHAIN_TYPES = new ConceptType[]
        {
            ConceptType.Person,
            ConceptType.Problem,
            ConceptType.Test,
            ConceptType.Treatment
        };

        private ConceptCollection _originalConcepts;
        private ObservableCollection<CorefChain> _editingChains;
        private ObservableCollection<Concept> _editingConcepts;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private bool _isRemovingConcepts;

        private IEnumerable<Concept> _displayConcepts;
        public IEnumerable<Concept> DisplayConcepts
        {
            get { return _displayConcepts; }
            set
            {
                if (SetProperty(ref _displayConcepts, value))
                {
                    ConceptsText = value != null
                        ? value.ToJointString(ConceptToString)
                        : EMPTY_EMR_CONCEPTS;
                }
            }
        }

        private string _conceptsText;
        public string ConceptsText
        {
            get { return _conceptsText; }
            set { SetProperty(ref _conceptsText, value); }
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

                    if (_editingChains != null)
                    {
                        CreateChainOrMergeCommand.RaiseCanExecuteChanged();                        
                    }

                    if (_editingConcepts != null)
                    {
                        RemoveConceptsCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public DelegateCommand SelectConceptCommand { get; }
        public DelegateCommand CreateChainOrMergeCommand { get; }
        public DelegateCommand RemoveConceptsCommand { get; }

        public InteractionRequest<ConceptTypeNotification> ChainTypeNotification { get; }

        [ImportingConstructor]
        public EMRConceptsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            SelectConceptCommand = new DelegateCommand(SelectConcepts);
            CreateChainOrMergeCommand = new DelegateCommand(CreateChainOrMerge, () => _editingChains != null && _focusedConcepts.Count > 0);
            RemoveConceptsCommand = new DelegateCommand(RemoveConcepts, () => _editingConcepts != null && _focusedConcepts.Count > 0);

            ChainTypeNotification = new InteractionRequest<ConceptTypeNotification>();

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(OnEMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<CorefAnnotationBegunEvent>().Subscribe(OnCorefAnnotationBegun);
            eventAggregator.GetEvent<CorefAnnotationEndedEvent>().Subscribe(OnCorefAnnotationEnded);
            eventAggregator.GetEvent<EntityAnnotationBegunEvent>().Subscribe(OnEntityAnnotationBegun);
            eventAggregator.GetEvent<EntityAnnotationEndedEvent>().Subscribe(OnEntityAnnotationEnded);

            _regionManager = regionManager;
        }

        private async void RemoveConcepts()
        {
            _isRemovingConcepts = true;
            foreach (var c in _focusedConcepts)
            {
                _editingConcepts.Remove(c);
            }
            ConceptsText = await _editingConcepts.ToJointStringAsync(ConceptToString);
            _isRemovingConcepts = false;
        }

        private void OnEntityAnnotationEnded(EMR resultEMR)
        {
            _editingConcepts.CollectionChanged -= EditingConcepts_CollectionChanged;
            _editingConcepts = null;
            DisplayConcepts = _originalConcepts = resultEMR.Concepts;
            RemoveConceptsCommand.RaiseCanExecuteChanged();
        }

        private void OnEntityAnnotationBegun(ObservableCollection<Concept> editingConcepts)
        {
            editingConcepts.CollectionChanged += EditingConcepts_CollectionChanged;
            DisplayConcepts = _editingConcepts = editingConcepts;
            RemoveConceptsCommand.RaiseCanExecuteChanged();
        }

        private async void EditingConcepts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_isRemovingConcepts)
            {
                ConceptsText = await _editingConcepts.ToJointStringAsync(ConceptToString);
            }
        }

        private void OnCorefAnnotationEnded(CorefChainCollection resultChains)
        {
            _editingChains = null;
            CreateChainOrMergeCommand.RaiseCanExecuteChanged();
        }

        private void CreateChainOrMerge()
        {
            CorefChain containedChain = null;
            int chainIndex = -1;

            foreach (var c in _focusedConcepts)
            {
                containedChain = _editingChains.FindChainContains(c, out chainIndex);
                if (containedChain != null)
                    break;
            }

            string output = null;
            if (containedChain != null)
            {
                var newChain = new List<Concept>(containedChain);
                newChain.AddRange(_focusedConcepts);
                _editingChains[chainIndex] = new CorefChain(newChain, containedChain.Type);
                output = $"{_focusedConcepts.Count} concepts has been added to the chain of type {containedChain.Type} at index {chainIndex}";
            }
            else if (_focusedConcepts.Count >= 2)
            {
                ChainTypeNotification.Raise(new ConceptTypeNotification()
                {
                    Title = "Create Chain",
                    Content = CHAIN_TYPES
                },
                n =>
                {
                    var newChain = new List<Concept>(_focusedConcepts);
                    _editingChains.Add(new CorefChain(newChain, n.SelectedType));
                    output = $"{_focusedConcepts.Count} concepts has been added to a new chain of type {n.SelectedType}";
                });
            }
            else
            {
                output = "There must be at least 2 concepts to create a new chain";
            }

            if (output != null)
            {
                _eventAggregator.GetEvent<OutputEvent>().Publish(output);
            }
        }

        private void OnCorefAnnotationBegun(ObservableCollection<CorefChain> editingChains)
        {
            _regionManager.RequestNavigate(RegionNames.Workspace, "EMRConceptsView");
            _editingChains = editingChains;
            CreateChainOrMergeCommand.RaiseCanExecuteChanged();
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

        private void OnEMRChanged(EMRChangedEventArgs e)
        {
            _originalConcepts = e?.EMR.Concepts;
            DisplayConcepts = _originalConcepts;
        }

        private static string ConceptToString(Concept c)
        {
            return $"{c}||t=\"{c.Type.ToString().ToLower()}\"";
        }
    }
}
