using HCMUT.EMRCorefResol;
using HCMUT.EMRCorefResol.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Stateless;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainViewModel : BindableBase
    {
        private readonly StateMachine<MainState, MainTrigger> _mainSM;
        private readonly IEventAggregator _eventAggregator;

        private EMR _currentEMR;
        private CorefChainCollection _currentGT;        
        private readonly IEMRReader _emrReader = new I2B2EMRReader();
        private readonly OutputEvent _outputEvent;

        private EntityAnnotator _entityAnnotator;
        private CorefAnnotator _corefAnnotator;

        private EMRCollection _emrCollection;
        private EMRCollection EMRCollection
        {
            get { return _emrCollection; }
            set
            {
                if (SetProperty(ref _emrCollection, value))
                {
                    OnPropertyChanged(nameof(TotalEMRCount));
                    _eventAggregator.GetEvent<EMRCollectionChangedEvent>().Publish(value);
                }
            }
        }

        private string _emrDirPath;
        public string EMRDirPath
        {
            get { return _emrDirPath; }
            private set
            {
                if (SetProperty(ref _emrDirPath, value))
                {
                    OpenEMRDirCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private int _currentEMRIndex = -1;
        public int CurrentEMRIndex
        {
            get { return _currentEMRIndex; }
            set { SetProperty(ref _currentEMRIndex, value); }
        }

        private bool _zeroBase = true;
        public bool ZeroBase
        {
            get { return _zeroBase; }
            set
            {
                if (SetProperty(ref _zeroBase, value) && _currentEMR != null)
                {                   
                    _currentEMR.BaseConceptIndex = value ? 0 : 1;
                    _eventAggregator.GetEvent<EMRBaseChangedEvent>().Publish(value);
                }
            }
        }

        public string CurrentState
        {
            get { return _mainSM.State.ToString(); }
        }

        public int TotalEMRCount
        {
            get { return _emrCollection?.Count - 1 ?? -1; }
        }

        public bool CanBeginAnnotate
        {
            get { return _mainSM.CanFire(MainTrigger.AnnotateCoref) || _mainSM.CanFire(MainTrigger.AnnotateEntity); }
        }

        public DelegateCommand GoNextCommand { get; }
        public DelegateCommand GoPreviousCommand { get; }
        public DelegateCommand GoToIndexCommand { get; }
        public DelegateCommand LoadCommand { get; }

        public DelegateCommand AnnotateCorefCommand { get; }
        public DelegateCommand AnnotateEntityCommand { get; }

        public DelegateCommand SaveAnnotationCommand { get; }
        public DelegateCommand CancelAnnotationCommand { get; }

        public DelegateCommand OpenEMRDirCommand { get; }

        public InteractionRequest<INotification> Notification { get; }
        public InteractionRequest<IConfirmation> Confirmation { get; }

        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator)
        {
            _mainSM = new StateMachine<MainState, MainTrigger>(MainState.NotLoaded);
            _mainSM.OnTransitioned(OnTransition);
            var loadWithState = _mainSM.SetTriggerParameters<MainState>(MainTrigger.Load);

            _mainSM.Configure(MainState.NotLoaded)
                .OnEntry(Unload)
                .OnEntry(OnStateReady)
                .PermitDynamic(loadWithState, Load);

            _mainSM.Configure(MainState.Ready)
                .OnEntry(OnReady)
                .OnEntry(OnStateReady)
                .PermitIf(MainTrigger.GoNext, MainState.Presenting, () => _currentEMRIndex < _emrCollection.Count - 1)
                .Permit(MainTrigger.GoToIndex, MainState.Presenting)
                .PermitDynamic(loadWithState, Load);

            _mainSM.Configure(MainState.Presenting)
                .OnEntryFrom(MainTrigger.GoNext, PresentNext)
                .OnEntryFrom(MainTrigger.GoPrevious, PresentPrevious)
                .OnEntryFrom(MainTrigger.GoToIndex, PresentIndex)
                .OnEntryFrom(MainTrigger.SaveAnnotation, SaveAnnotation)
                .OnEntryFrom(MainTrigger.CancelAnnotation, CancelAnnotation)
                .OnEntry(OnStateReady)
                .PermitReentryIf(MainTrigger.GoNext, () => _currentEMRIndex < _emrCollection.Count - 1)
                .PermitReentryIf(MainTrigger.GoPrevious, () => _currentEMRIndex > 0)
                .PermitReentry(MainTrigger.GoToIndex)
                .PermitDynamic(loadWithState, Load)
                .Permit(MainTrigger.AnnotateEntity, MainState.EntityAnnotating)
                .Permit(MainTrigger.AnnotateCoref, MainState.CorefAnnotating);

            _mainSM.Configure(MainState.CorefAnnotating)
                .OnEntry(BeginCorefAnnotation)
                .OnEntry(OnStateReady)
                .Permit(MainTrigger.SaveAnnotation, MainState.Presenting)
                .Permit(MainTrigger.CancelAnnotation, MainState.Presenting);

            _mainSM.Configure(MainState.EntityAnnotating)
                .OnEntry(BeginEntityAnnotation)
                .OnEntry(OnStateReady)
                .Permit(MainTrigger.SaveAnnotation, MainState.Presenting)
                .Permit(MainTrigger.CancelAnnotation, MainState.Presenting);

            LoadCommand = _mainSM.TriggerToCommand(loadWithState, () => _mainSM.State);
            GoNextCommand = _mainSM.TriggerToCommand(MainTrigger.GoNext);
            GoToIndexCommand = _mainSM.TriggerToCommand(MainTrigger.GoToIndex);
            GoPreviousCommand = _mainSM.TriggerToCommand(MainTrigger.GoPrevious);

            AnnotateCorefCommand = _mainSM.TriggerToCommand(MainTrigger.AnnotateCoref);
            AnnotateEntityCommand = _mainSM.TriggerToCommand(MainTrigger.AnnotateEntity);

            SaveAnnotationCommand = _mainSM.TriggerToCommand(MainTrigger.SaveAnnotation);
            CancelAnnotationCommand = _mainSM.TriggerToCommand(MainTrigger.CancelAnnotation);

            OpenEMRDirCommand = new DelegateCommand(OpenEMRDir, () => Directory.Exists(_emrDirPath));

            Notification = new InteractionRequest<INotification>();
            Confirmation = new InteractionRequest<IConfirmation>();

            _eventAggregator = eventAggregator;
            _outputEvent = eventAggregator.GetEvent<OutputEvent>();
            eventAggregator.GetEvent<EMRIndexChangedEvent>().Subscribe(OnEMRIndexChanged, ThreadOption.UIThread);
        }

        private void OpenEMRDir()
        {
            Process.Start(_emrDirPath);
        }

        private void OnEMRIndexChanged(int index)
        {
            if (_mainSM.CanFire(MainTrigger.GoToIndex))
            {
                CurrentEMRIndex = index;
                _mainSM.Fire(MainTrigger.GoToIndex);
            }
        }

        private void OnTransition(StateMachine<MainState, MainTrigger>.Transition t)
        {
            var text = $"{t.Trigger}: {t.Source} -> {t.Destination} (re-entry: {t.IsReentry})";
            _outputEvent.Publish(text);
        }

        private void CancelAnnotation(StateMachine<MainState, MainTrigger>.Transition transition)
        {
            switch (transition.Source)
            {
                case MainState.CorefAnnotating:
                    _corefAnnotator = null;
                    _eventAggregator.GetEvent<CorefAnnotationEndedEvent>().Publish(_currentGT);
                    break;
                case MainState.EntityAnnotating:
                    _entityAnnotator = null;
                    _eventAggregator.GetEvent<EntityAnnotationEndedEvent>().Publish(
                        new EntityAnnotationEndedEventArgs(_currentEMR, _currentGT));
                    break;
            }
        }

        private async void SaveAnnotation(StateMachine<MainState, MainTrigger>.Transition transition)
        {
            // TODO: check if ground truth or concepts is saved properly, we assume they are for now
            switch (transition.Source)
            {
                case MainState.CorefAnnotating:
                    {
                        var savingTask = Task.Run(() =>
                        {
                            var chainsPath = _emrCollection.GetChainsPath(_currentEMRIndex);
                            var chainsFolder = Path.GetDirectoryName(chainsPath);
                            var editingChains = _corefAnnotator.EditingChains;

                            Directory.CreateDirectory(chainsFolder);
                            File.WriteAllLines(chainsPath, editingChains.Select(c => c.ToString()));

                            return new CorefChainCollection(editingChains.ToList());
                        });

                        _currentGT = await savingTask;
                        _corefAnnotator = null;
                        _eventAggregator.GetEvent<CorefAnnotationEndedEvent>().Publish(_currentGT);
                    }
                    break;
                case MainState.EntityAnnotating:
                    {
                        var savingTask = Task.Run(() =>
                        {
                            var editingConcepts = _entityAnnotator.EditingConcepts;
                            var conceptsPath = _emrCollection.GetConceptsPath(_currentEMRIndex);
                            File.WriteAllLines(conceptsPath, editingConcepts.Select(c => $"{c}||t=\"{c.Type.ToString().ToLower()}\""));

                            var editingChains = _entityAnnotator.EditingChains;
                            var chainsPath = _emrCollection.GetChainsPath(_currentEMRIndex);
                            var chainsFolder = Path.GetDirectoryName(chainsPath);

                            Directory.CreateDirectory(chainsFolder);
                            File.WriteAllLines(chainsPath, editingChains.Select(c => c.ToString()));

                            var emrPath = _emrCollection.GetEMRPath(_currentEMRIndex);
                            var resultEMR = new EMR(emrPath, conceptsPath, _emrReader);
                            resultEMR.BaseConceptIndex = _zeroBase ? 0 : 1;

                            var resultChains = new CorefChainCollection(editingChains.ToList());
                            return new EntityAnnotationEndedEventArgs(resultEMR, resultChains);
                        });

                        var e = await savingTask;
                        _currentEMR = e.ResultEMR;
                        _currentGT = e.ResultChains;

                        _entityAnnotator = null;
                        _eventAggregator.GetEvent<EntityAnnotationEndedEvent>().Publish(e);
                    }
                    break;
            }
        }

        private void BeginEntityAnnotation()
        {
            if (_currentEMR.Concepts.Count > 0)
            {
                Confirmation.Raise(new Confirmation(NotificationType.None)
                {
                    Title = "Annotate Entity",
                    Content = "Do you want to override existing concepts?"
                },
                c => DoEntityAnnotation(c.Confirmed));
            }
            else
            {
                DoEntityAnnotation(true);
            }
        }

        private void DoEntityAnnotation(bool shouldOverride)
        {
            var editingConcepts = shouldOverride ? new List<Concept>()
                : new List<Concept>(_currentEMR.Concepts);
            var editingChains = (_currentGT == null || _currentGT.Count == 0 || shouldOverride) ? new List<CorefChain>()
                : new List<CorefChain>(_currentGT);

            _entityAnnotator = new EntityAnnotator(editingConcepts, editingChains);
            _eventAggregator.GetEvent<EntityAnnotationBegunEvent>()
                .Publish(_entityAnnotator);
        }

        private void BeginCorefAnnotation()
        {
            if (_currentGT != null && _currentGT.Count > 0)
            {
                Confirmation.Raise(new Confirmation(NotificationType.None)
                {
                    Title = "Annotate Coreference",
                    Content = "Do you want to override existing ground truth?"
                }, 
                c => DoCorefAnnotation(c.Confirmed));
            }
            else
            {
                DoCorefAnnotation(true);
            }
        }

        private void DoCorefAnnotation(bool shouldOverride)
        {
            var editingChains = shouldOverride ? new List<CorefChain>() 
                : new List<CorefChain>(_currentGT);

            _corefAnnotator = new CorefAnnotator(editingChains);
            _eventAggregator.GetEvent<CorefAnnotationBegunEvent>().Publish(_corefAnnotator);
        }

        private void Unload()
        {
            EMRCollection = null;
            EMRDirPath = null;
            CurrentEMRIndex = -1;

            _currentEMR = null;
            _currentGT = null;
        }

        private MainState Load(MainState currentState)
        {
            using (var d = new CommonOpenFileDialog())
            {
                d.IsFolderPicker = true;
                d.Multiselect = false;

                if (d.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var path = d.FileName;
                    if (string.Equals(path, _emrDirPath))
                    {
                        return currentState;
                    }
                    else if (EMRCollection.IsPathOk(path))
                    {
                        EMRDirPath = path;
                        return MainState.Ready;
                    }
                    else
                    {
                        Notification.Raise(new Notification(NotificationType.Information)
                        {
                            Title = "Load EMR Directory",
                            Content = $"Cannot load. The directory '{d.FileName}' is not a proper EMR directory."
                        });
                    }
                }

                return currentState;
            }
        }

        private void OnReady()
        {
            EMRCollection = new EMRCollection(_emrDirPath);
            CurrentEMRIndex = -1;
            BroadcastCurrentEMR();
        }

        private void PresentNext()
        {
            CurrentEMRIndex = Math.Min(CurrentEMRIndex + 1, _emrCollection.Count - 1);
            BroadcastCurrentEMR();
        }

        private void PresentPrevious()
        {
            CurrentEMRIndex = Math.Max(CurrentEMRIndex - 1, 0);
            BroadcastCurrentEMR();
        }

        private void PresentIndex()
        {
            BroadcastCurrentEMR();
        }

        private void BroadcastCurrentEMR()
        {
            if (_emrCollection != null)
            {
                if (_currentEMRIndex >= 0 && _currentEMRIndex < _emrCollection.Count)
                {
                    var emrPath = _emrCollection.GetEMRPath(_currentEMRIndex);
                    var conceptsPath = _emrCollection.GetConceptsPath(_currentEMRIndex);

                    _currentEMR = new EMR(emrPath, conceptsPath, _emrReader);
                    _currentEMR.BaseConceptIndex = _zeroBase ? 0 : 1;

                    var gtPath = _emrCollection.GetChainsPath(_currentEMRIndex);
                    _currentGT = File.Exists(gtPath) ? new CorefChainCollection(gtPath, _emrReader) : null;

                    var e = new EMRChangedEventArgs(_currentEMR, _currentGT);
                    _eventAggregator.GetEvent<EMRChangedEvent>().Publish(e);
                    return;
                }
            }

            _eventAggregator.GetEvent<EMRChangedEvent>().Publish(null);
        }

        private void UpdateCommandStates()
        {
            GoNextCommand.RaiseCanExecuteChanged();
            GoPreviousCommand.RaiseCanExecuteChanged();
            GoToIndexCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();

            AnnotateCorefCommand.RaiseCanExecuteChanged();
            AnnotateEntityCommand.RaiseCanExecuteChanged();

            SaveAnnotationCommand.RaiseCanExecuteChanged();
            CancelAnnotationCommand.RaiseCanExecuteChanged();
        }

        private void OnStateReady()
        {
            UpdateCommandStates();
            OnPropertyChanged(nameof(CurrentState));
            OnPropertyChanged(nameof(CanBeginAnnotate));
        }
    }
}
