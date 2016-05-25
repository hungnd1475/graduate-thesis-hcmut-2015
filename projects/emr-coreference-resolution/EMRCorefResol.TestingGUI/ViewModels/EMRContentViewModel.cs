using HCMUT.EMRCorefResol;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.ComponentModel.Composition;
using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Prism.Interactivity.InteractionRequest;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EMRContentViewModel : BindableBase
    {
        private static readonly ConceptType[] CONCEPT_TYPES = new ConceptType[]
        {
            ConceptType.Person,
            ConceptType.Problem,
            ConceptType.Test,
            ConceptType.Treatment,
            ConceptType.Pronoun
        };

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private EntityAnnotator _entityAnnotator;        

        private EMR _currentEMR;
        public EMR CurrentEMR
        {
            get { return _currentEMR; }
            set { SetProperty(ref _currentEMR, value); }
        }

        private IReadOnlyList<Concept> _selectedConcepts;
        public IReadOnlyList<Concept> SelectedConcepts
        {
            get { return _selectedConcepts; }
            set { SetProperty(ref _selectedConcepts, value); }
        }

        private TextSelectionInfo _textSelection;
        public TextSelectionInfo TextSelection
        {
            get { return _textSelection; }
            set
            {
                if (SetProperty(ref _textSelection, value) && _entityAnnotator != null)
                {
                    AnnotateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand AnnotateCommand { get; }

        public InteractionRequest<ConceptTypeNotification> ConceptTypeNotification { get; }

        [ImportingConstructor]
        public EMRContentViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            CurrentEMR = null;
            AnnotateCommand = new DelegateCommand(Annotate, CanAnnotate);
            ConceptTypeNotification = new InteractionRequest<ConceptTypeNotification>();

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(EMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<SelectedConceptsChangedEvent>().Subscribe(SelectedConceptChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<EMRBaseChangedEvent>().Subscribe(OnEMRBaseChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<EntityAnnotationBegunEvent>().Subscribe(OnEntityAnnotationBegun, ThreadOption.UIThread);
            eventAggregator.GetEvent<EntityAnnotationEndedEvent>().Subscribe(OnEntityAnnotationEnded, ThreadOption.UIThread);

            _regionManager = regionManager;
        }

        private void OnEntityAnnotationBegun(EntityAnnotator entityAnnotator)
        {
            _regionManager.RequestNavigate(RegionNames.Workspace, "EMRContentView");
            _entityAnnotator = entityAnnotator;
            _entityAnnotator.OperationCompleted += EntityAnnotator_OperationCompleted;
            AnnotateCommand.RaiseCanExecuteChanged();
        }

        private void EntityAnnotator_OperationCompleted(EntityAnnotator annotator,
            AnnotationOperationCompletedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Message))
            {
                _eventAggregator.GetEvent<OutputEvent>().Publish(e.Message);
            }
        }

        private void OnEntityAnnotationEnded(EntityAnnotationEndedEventArgs e)
        {
            _entityAnnotator.OperationCompleted -= EntityAnnotator_OperationCompleted;
            _entityAnnotator = null;
            CurrentEMR = e.ResultEMR;
            AnnotateCommand.RaiseCanExecuteChanged();
        }

        private void Annotate()
        {
            ConceptTypeNotification.Raise(new ConceptTypeNotification()
            {
                Title = "Annotate Concepts",
                Content = CONCEPT_TYPES
            },
            c =>
            {
                var concept = GetConceptFromSelection(c.SelectedType);
                _entityAnnotator.AddConcept(concept);
            });
        }

        private bool CanAnnotate()
        {
            return _currentEMR != null
                && _entityAnnotator != null
                && !string.IsNullOrWhiteSpace(_textSelection.Text);
        }

        private Concept GetConceptFromSelection(ConceptType type)
        {
            var text = _textSelection.Text;
            var startIndex = _textSelection.StartColumn - 1;
            var endIndex = _textSelection.EndColumn - 1;

            if (char.IsWhiteSpace(text[0]))
            {
                startIndex += 1;
            }
            if (char.IsWhiteSpace(text[text.Length - 1]))
            {
                endIndex -= 1;
            }

            var startLine = _textSelection.StartLine;
            var startLineText = _currentEMR.GetLine(startLine);
            var startWordIndex = startLineText.Substring(0, startIndex).Count(c => char.IsWhiteSpace(c)) + _currentEMR.BaseConceptIndex;

            var endLine = _textSelection.EndLine;
            var endLineText = endLine != startLine ? _currentEMR.GetLine(endLine) : startLineText;
            var endWordIndex = endLineText.Substring(0, endIndex).Count(c => char.IsWhiteSpace(c)) + _currentEMR.BaseConceptIndex;

            return new Concept(text.Trim(),
                new ConceptPosition(startLine, startWordIndex),
                new ConceptPosition(endLine, endWordIndex),
                type);
        }

        private void OnEMRBaseChanged(bool zeroBase)
        {
            if (_selectedConcepts != null)
            {
                var t = _selectedConcepts;
                SelectedConcepts = null;
                SelectedConcepts = t;
            }
        }

        private void SelectedConceptChanged(IReadOnlyList<Concept> selectedConcepts)
        {
            SelectedConcepts = selectedConcepts;
        }

        private void EMRChanged(EMRChangedEventArgs e)
        {
            CurrentEMR = e?.EMR;
        }
    }
}
