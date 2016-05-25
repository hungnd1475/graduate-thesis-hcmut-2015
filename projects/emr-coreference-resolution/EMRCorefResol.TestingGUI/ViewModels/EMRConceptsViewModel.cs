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
        private static readonly ConceptType[] CHAIN_TYPES = new ConceptType[]
        {
            ConceptType.Person,
            ConceptType.Problem,
            ConceptType.Test,
            ConceptType.Treatment
        };
        private static readonly ConceptType[] CONCEPT_TYPES = new ConceptType[]
        {
            ConceptType.Person,
            ConceptType.Problem,
            ConceptType.Test,
            ConceptType.Treatment,
            ConceptType.Pronoun            
        };

        private ConceptCollection _originalConcepts;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private EntityAnnotator _entityAnnotator;
        private CorefAnnotator _corefAnnotator;

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
                    _eventAggregator.GetEvent<SelectedConceptsChangedEvent>().Publish(null);

                    if (_entityAnnotator != null)
                    {
                        RemoveConceptsCommand.RaiseCanExecuteChanged();
                        ChangeConceptTypeCommand.RaiseCanExecuteChanged();
                    }

                    if (_corefAnnotator != null)
                    {
                        CreateChainOrMergeCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        private CorefChain _selectedChain;
        public CorefChain SelectedChain
        {
            get { return _selectedChain; }
            set { SetProperty(ref _selectedChain, value); }
        }

        public DelegateCommand SelectConceptsCommand { get; }
        public DelegateCommand CreateChainOrMergeCommand { get; }
        public DelegateCommand RemoveConceptsCommand { get; }
        public DelegateCommand ChangeConceptTypeCommand { get; }

        public InteractionRequest<ConceptTypeNotification> TypeNotification { get; }

        [ImportingConstructor]
        public EMRConceptsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            SelectConceptsCommand = new DelegateCommand(SelectConcepts);
            CreateChainOrMergeCommand = new DelegateCommand(CreateChainOrMerge, 
                () => _corefAnnotator != null && _focusedConcepts.Count >= 2);
            RemoveConceptsCommand = new DelegateCommand(RemoveConcepts, 
                () => _entityAnnotator != null && _focusedConcepts.Count > 0);
            ChangeConceptTypeCommand = new DelegateCommand(ChangeConceptType,
                () => _entityAnnotator != null && _focusedConcepts.Count == 1);

            TypeNotification = new InteractionRequest<ConceptTypeNotification>();

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(OnEMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<CorefAnnotationBegunEvent>().Subscribe(OnCorefAnnotationBegun);
            eventAggregator.GetEvent<CorefAnnotationEndedEvent>().Subscribe(OnCorefAnnotationEnded);
            eventAggregator.GetEvent<EntityAnnotationBegunEvent>().Subscribe(OnEntityAnnotationBegun);
            eventAggregator.GetEvent<EntityAnnotationEndedEvent>().Subscribe(OnEntityAnnotationEnded);
            eventAggregator.GetEvent<SelectedChainChangedEvent>().Subscribe(SelectedChainChanged);

            _regionManager = regionManager;
        }

        private void SelectedChainChanged(CorefChain chain)
        {
            SelectedChain = chain;
        }

        private void ChangeConceptType()
        {
            TypeNotification.Raise(new ConceptTypeNotification()
            {
                Title = "Change Concept Type",
                Content = CONCEPT_TYPES
            },
            async n =>
            {
                var concept = _focusedConcepts[0];
                if (await _entityAnnotator.ChangeConceptType(concept, n.SelectedType))
                {
                    var message = $"Concept '{concept}' has been changed to type {n.SelectedType}";
                    _eventAggregator.GetEvent<OutputEvent>().Publish(message);
                }         
            });
        }

        private async void RemoveConcepts()
        {
            await _entityAnnotator.RemoveConceptsAsync(_focusedConcepts);
        }

        private async void OnEntityAnnotationEnded(EntityAnnotationEndedEventArgs e)
        {
            _entityAnnotator.OperationCompleted -= EntityAnnotator_OperationCompleted;
            _entityAnnotator = null;                        
            _originalConcepts = e.ResultEMR.Concepts;

            ConceptsText = await _originalConcepts.ToJointStringAsync(c => c.ToString(true));
            RemoveConceptsCommand.RaiseCanExecuteChanged();
            ChangeConceptTypeCommand.RaiseCanExecuteChanged();
        }

        private async void OnEntityAnnotationBegun(EntityAnnotator entityAnnotator)
        {
            _entityAnnotator = entityAnnotator;
            _entityAnnotator.OperationCompleted += EntityAnnotator_OperationCompleted;

            ConceptsText = await entityAnnotator.EditingConcepts.ToJointStringAsync(c => c.ToString(true));
            RemoveConceptsCommand.RaiseCanExecuteChanged();
            ChangeConceptTypeCommand.RaiseCanExecuteChanged();
        }

        private async void EntityAnnotator_OperationCompleted(EntityAnnotator entityAnnotator,
            AnnotationOperationCompletedEventArgs e)
        {
            if (e.Result == AnnotationOperationResult.Changed)
            {
                ConceptsText = await entityAnnotator.EditingConcepts.ToJointStringAsync(c => c.ToString(true));
            }
        }

        private void OnCorefAnnotationBegun(CorefAnnotator corefAnnotator)
        {
            _regionManager.RequestNavigate(RegionNames.Workspace, "EMRConceptsView");
            _corefAnnotator = corefAnnotator;
            //_corefAnnotator.OperationCompleted += CorefAnnotator_OperationCompleted;
            CreateChainOrMergeCommand.RaiseCanExecuteChanged();
        }

        //private void CorefAnnotator_OperationCompleted(AnnotationOperationCompletedEventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(e.Message))
        //    {
        //        _eventAggregator.GetEvent<OutputEvent>().Publish(e.Message);
        //    }
        //}

        private void OnCorefAnnotationEnded(CorefChainCollection resultChains)
        {
            //_corefAnnotator.OperationCompleted -= CorefAnnotator_OperationCompleted;
            _corefAnnotator = null;
            CreateChainOrMergeCommand.RaiseCanExecuteChanged();
        }

        private async void CreateChainOrMerge()
        {
            var mergedIndex = await _corefAnnotator.MergeChainAsync(_focusedConcepts);
            if (mergedIndex >= 0)
            {
                var message = $"{_focusedConcepts.Count} concepts has been merged to the chain at index {mergedIndex}";
                _eventAggregator.GetEvent<OutputEvent>().Publish(message);
            }
            else
            {
                //TypeNotification.Raise(new ConceptTypeNotification()
                //{
                //    Title = "Create Chain",
                //    Content = CHAIN_TYPES
                //},
                //async n =>
                //{

                //});
                var newChainType = await _corefAnnotator.CreateChainAsync(_focusedConcepts);
                var message = string.Empty;
                if (newChainType == ConceptType.None)
                {
                    message = "Cannot create new chain: There are two concepts with different types.";
                }
                else if (newChainType == ConceptType.Pronoun)
                {
                    message = "Cannot create new chain: All concepts are pronouns";
                }
                else
                {
                    message = $"{_focusedConcepts.Count} concepts has been added to a new chain of type {newChainType}";
                }
                _eventAggregator.GetEvent<OutputEvent>().Publish(message);
            }                     
        }

        private void SelectConcepts()
        {
            _eventAggregator.GetEvent<SelectedConceptsChangedEvent>().Publish(_focusedConcepts);
        }

        private async void OnEMRChanged(EMRChangedEventArgs e)
        {
            _originalConcepts = e?.EMR.Concepts;
            ConceptsText = await _originalConcepts.ToJointStringAsync(c => c.ToString(true));
        }
    }
}
