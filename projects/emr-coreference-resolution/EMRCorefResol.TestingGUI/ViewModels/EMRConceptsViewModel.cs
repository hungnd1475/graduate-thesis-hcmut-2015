using HCMUT.EMRCorefResol;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EMRConceptsViewModel : DockableContentViewModel
    {
        public static readonly string ID = "EMRConcepts";
        private static readonly string EMPTY_EMR_CONCEPTS = "<empty>";

        private ConceptCollection _originalConcepts;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private IEnumerable<Concept> _displayConcepts;
        public IEnumerable<Concept> DisplayConcepts
        {
            get { return _displayConcepts; }
            set
            {
                if (SetProperty(ref _displayConcepts, value))
                {
                    ConceptsText = value != null 
                        ? string.Join(Environment.NewLine, value.Select(c => $"{c}||t=\"{c.Type.ToString().ToLower()}\""))
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
                    CreateChainOrMergeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isAnnotating;
        public bool IsAnnotating
        {
            get { return _isAnnotating; }
            set
            {
                if (SetProperty(ref _isAnnotating, value))
                {
                    CreateChainOrMergeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand SelectConceptCommand { get; }
        public DelegateCommand CreateChainOrMergeCommand { get; }

        [ImportingConstructor]
        public EMRConceptsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
            : base(ID)
        {
            Title = "Concepts";
            SelectConceptCommand = new DelegateCommand(SelectConcepts);
            CreateChainOrMergeCommand = new DelegateCommand(CreateChainOrMerge, () => _isAnnotating && _focusedConcepts.Count > 0);

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(OnEMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<AnnotationBegunEvent>().Subscribe(OnAnnotationBegun);
            eventAggregator.GetEvent<AnnotationEndedEvent>().Subscribe(OnAnnotationEnded);

            _regionManager = regionManager;
        }

        private void OnAnnotationEnded(CorefChainCollection resultChains)
        {
            IsAnnotating = false;
        }

        private void CreateChainOrMerge()
        {
            _eventAggregator.GetEvent<CreateChainOrMergeEvent>().Publish(_focusedConcepts);
        }

        private void OnAnnotationBegun(ObservableCollection<CorefChain> editingChains)
        {
            IsAnnotating = true;
            _regionManager.RequestNavigate(RegionNames.Workspace, "EMRConceptsView");
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
    }
}
