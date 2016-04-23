using HCMUT.EMRCorefResol;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GroundTruthViewModel : DockableContentViewModel
    {
        public static readonly string ID = "GroundTruth";

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private ObservableCollection<CorefChain> _editingChains;

        private IEnumerable<CorefChain> _groundTruth;
        private IEnumerable<CorefChain> GroundTruth
        {
            get { return _groundTruth; }
            set
            {
                if (SetProperty(ref _groundTruth, value) && value != null)
                {
                    GTText = string.Join(Environment.NewLine, value.Select(c => c.ToString()));
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
                }
            }
        }

        public DelegateCommand SelectConceptCommand { get; }
        public InteractionRequest<ChainTypeNotification> TypeChoosingRequest { get; }

        [ImportingConstructor]
        public GroundTruthViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
            : base(ID)
        {
            Title = "Ground Truth";
            SelectConceptCommand = new DelegateCommand(SelectConcepts);
            TypeChoosingRequest = new InteractionRequest<ChainTypeNotification>();

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(EMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<AnnotationBegunEvent>().Subscribe(OnAnnotationBegun, ThreadOption.UIThread);
            eventAggregator.GetEvent<CreateChainOrMergeEvent>().Subscribe(OnCreateChainOrMerge, ThreadOption.UIThread);
            eventAggregator.GetEvent<AnnotationEndedEvent>().Subscribe(OnAnnotationEnded, ThreadOption.UIThread);

            _regionManager = regionManager;
        }

        private void OnAnnotationEnded(CorefChainCollection resultChains)
        {
            _editingChains.CollectionChanged -= _editingChains_CollectionChanged;
            _editingChains = null;
            GroundTruth = resultChains;
        }

        private void OnCreateChainOrMerge(IEnumerable<Concept> concepts)
        {
            if (_editingChains != null)
            {
                _regionManager.RequestNavigate(RegionNames.Workspace, "GroundTruthView");

                CorefChain containedChain = null;
                int chainIndex = -1;

                foreach (var c in concepts)
                {
                    containedChain = _editingChains.FindChainContains(c, out chainIndex);
                    if (containedChain != null)
                        break;
                }

                if (containedChain != null)
                {
                    var newChain = new List<Concept>(containedChain);
                    newChain.AddRange(concepts);
                    _editingChains[chainIndex] = new CorefChain(newChain, containedChain.Type);
                }
                else
                {
                    TypeChoosingRequest.Raise(new ChainTypeNotification()
                    {
                        Title = "Choose chain type",
                        Content = Enum.GetValues(typeof(ConceptType)).Cast<ConceptType>()
                    },
                    n =>
                    {
                        var newChain = new List<Concept>(concepts);
                        _editingChains.Add(new CorefChain(newChain, n.SelectedType));
                    });
                }
            }
        }

        private void OnAnnotationBegun(ObservableCollection<CorefChain> editingChains)
        {
            GroundTruth = _editingChains = editingChains;
            _editingChains.CollectionChanged += _editingChains_CollectionChanged;
        }

        private void _editingChains_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GTText = string.Join(Environment.NewLine, _editingChains.Select(c => c.ToString()));
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
