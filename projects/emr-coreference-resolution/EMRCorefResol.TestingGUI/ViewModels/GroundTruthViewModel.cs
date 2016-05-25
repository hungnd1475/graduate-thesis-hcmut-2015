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
        private CorefAnnotator _corefAnnotator;
        private EntityAnnotator _entityAnnotator;
        private CorefChainCollection _groundTruth;

        //private IEnumerable<CorefChain> _groundTruth;
        //private IEnumerable<CorefChain> GroundTruth
        //{
        //    get { return _groundTruth; }
        //    set
        //    {
        //        if (SetProperty(ref _groundTruth, value))
        //        {
        //            GTText = value.ToJointString();
        //        }
        //    }
        //}

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
                    _eventAggregator.GetEvent<SelectedConceptsChangedEvent>().Publish(null);
                    RemoveConceptsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand SelectConceptsCommand { get; }
        public DelegateCommand RemoveConceptsCommand { get; }
        public DelegateCommand<int?> SelectChainCommand { get; }

        [ImportingConstructor]
        public GroundTruthViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            SelectConceptsCommand = new DelegateCommand(SelectConcepts);
            RemoveConceptsCommand = new DelegateCommand(RemoveConcepts,
                () => _corefAnnotator != null && _focusedConcepts != null && _focusedConcepts.Count > 0);
            SelectChainCommand = new DelegateCommand<int?>(SelectChain);

            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(EMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<CorefAnnotationBegunEvent>().Subscribe(OnCorefAnnotationBegun, ThreadOption.UIThread);
            eventAggregator.GetEvent<CorefAnnotationEndedEvent>().Subscribe(OnCorefAnnotationEnded, ThreadOption.UIThread);
            eventAggregator.GetEvent<EntityAnnotationBegunEvent>().Subscribe(OnEntityAnnotationBegun, ThreadOption.UIThread);
            eventAggregator.GetEvent<EntityAnnotationEndedEvent>().Subscribe(OnEntityAnnotationEnded, ThreadOption.UIThread);

            _regionManager = regionManager;
        }

        private void SelectChain(int? index)
        {
            if (index.HasValue)
            {
                var chain = GetChain(index.Value);
                _eventAggregator.GetEvent<SelectedChainChangedEvent>().Publish(chain);
            }
        }

        private CorefChain GetChain(int index)
        {
            IReadOnlyList<CorefChain> editingChains = null;
            if (_corefAnnotator != null)
            {
                editingChains = _corefAnnotator.EditingChains;
            }
            else if (_entityAnnotator != null)
            {
                editingChains = _entityAnnotator.EditingChains;
            }

            if (editingChains != null)
            {
                return index >= 0 && index < editingChains.Count ?
                    editingChains[index] : null;
            }
            else if (_groundTruth != null)
            {
                return index >= 0 && index < _groundTruth.Count ?
                    _groundTruth[index] : null;
            }
            else
            {
                return null;
            }
        }

        private async void OnEntityAnnotationEnded(EntityAnnotationEndedEventArgs e)
        {
            _entityAnnotator.CorefOperationCompleted -= CorefAnnotator_OperationCompleted;
            _entityAnnotator = null;
            _groundTruth = e.ResultChains;
            GTText = await e.ResultChains.ToJointStringAsync();
        }

        private async void OnEntityAnnotationBegun(EntityAnnotator entityAnnotator)
        {
            _entityAnnotator = entityAnnotator;
            _entityAnnotator.CorefOperationCompleted += CorefAnnotator_OperationCompleted;
            _groundTruth = null;
            GTText = await _entityAnnotator.EditingChains.ToJointStringAsync();
        }

        private async void RemoveConcepts()
        {
            await _corefAnnotator.RemoveConceptsAsync(_focusedConcepts);
        }

        private async void OnCorefAnnotationEnded(CorefChainCollection resultChains)
        {
            _corefAnnotator.OperationCompleted -= CorefAnnotator_OperationCompleted;
            _corefAnnotator = null;
            _groundTruth = resultChains;

            GTText = await resultChains.ToJointStringAsync();
            RemoveConceptsCommand.RaiseCanExecuteChanged();
        }

        private async void OnCorefAnnotationBegun(CorefAnnotator corefAnnotator)
        {
            _corefAnnotator = corefAnnotator;
            _corefAnnotator.OperationCompleted += CorefAnnotator_OperationCompleted;
            _groundTruth = null;

            GTText = await corefAnnotator.EditingChains.ToJointStringAsync();
            RemoveConceptsCommand.RaiseCanExecuteChanged();
        }

        private async void CorefAnnotator_OperationCompleted(CorefAnnotator corefAnnotator,
            AnnotationOperationCompletedEventArgs e)
        {
            if (e.Result == AnnotationOperationResult.Changed)
            {
                GTText = await corefAnnotator.EditingChains.ToJointStringAsync();
            }
        }

        private void SelectConcepts()
        {
            _eventAggregator.GetEvent<SelectedConceptsChangedEvent>().Publish(_focusedConcepts);
        }

        private async void EMRChanged(EMRChangedEventArgs e)
        {
            _groundTruth = e?.GroundTruth;
            GTText = await _groundTruth.ToJointStringAsync();
        }
    }
}
