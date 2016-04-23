using HCMUT.EMRCorefResol;
using HCMUT.EMRCorefResol.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Stateless;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainViewModel : BindableBase
    {
        private readonly StateMachine<MainState, MainTrigger> _mainSM;
        private readonly IEventAggregator _eventAggregator;

        private EMRCollection _emrCollection;
        private EMR _currentEMR;
        private CorefChainCollection _currentGT;
        private ObservableCollection<CorefChain> _editingChains;
        private readonly IEMRReader _emrReader = new I2B2EMRReader();

        private string _emrDirPath;
        public string EMRDirPath
        {
            get { return _emrDirPath; }
            private set { SetProperty(ref _emrDirPath, value); }
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

        public DelegateCommand GoNextCommand { get; }
        public DelegateCommand GoPreviousCommand { get; }
        public DelegateCommand GoToIndexCommand { get; }
        public DelegateCommand LoadCommand { get; }

        public DelegateCommand AnnotateCommand { get; }
        public DelegateCommand SaveAnnotationCommand { get; }
        public DelegateCommand CancelAnnotationCommand { get; }

        public InteractionRequest<INotification> Notification { get; }
        public InteractionRequest<IConfirmation> Confirmation { get; }

        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _mainSM = new StateMachine<MainState, MainTrigger>(MainState.NotLoaded);

            var loadWithState = _mainSM.SetTriggerParameters<MainState>(MainTrigger.Load);
            var goToIndex = _mainSM.SetTriggerParameters<int>(MainTrigger.GoToIndex);

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
                .Permit(MainTrigger.Annotate, MainState.Annotating);

            _mainSM.Configure(MainState.Annotating)
                .OnEntry(BeginAnnotation)
                .OnEntry(OnStateReady)
                .Permit(MainTrigger.SaveAnnotation, MainState.Presenting)
                .Permit(MainTrigger.CancelAnnotation, MainState.Presenting);

            LoadCommand = _mainSM.TriggerToCommand(loadWithState, () => _mainSM.State);
            GoNextCommand = _mainSM.TriggerToCommand(MainTrigger.GoNext);
            GoToIndexCommand = _mainSM.TriggerToCommand(MainTrigger.GoToIndex);
            GoPreviousCommand = _mainSM.TriggerToCommand(MainTrigger.GoPrevious);

            AnnotateCommand = _mainSM.TriggerToCommand(MainTrigger.Annotate);
            SaveAnnotationCommand = _mainSM.TriggerToCommand(MainTrigger.SaveAnnotation);
            CancelAnnotationCommand = _mainSM.TriggerToCommand(MainTrigger.CancelAnnotation);

            Notification = new InteractionRequest<INotification>();
            Confirmation = new InteractionRequest<IConfirmation>();
        }

        private void CancelAnnotation()
        {
            _editingChains = null;
            _eventAggregator.GetEvent<AnnotationEndedEvent>().Publish(_currentGT);
        }

        private void SaveAnnotation()
        {
            // TODO: check if ground truth is saved properly, we assume it is for now

            var chainPath = _emrCollection.GetChainsPath(_currentEMRIndex);
            var chainFolder = Path.GetDirectoryName(chainPath);
            Directory.CreateDirectory(chainFolder);
            File.WriteAllLines(chainPath, _editingChains.Select(c => c.ToString()));
            
            _currentGT = new CorefChainCollection(_editingChains.ToList());
            _editingChains = null;
            _eventAggregator.GetEvent<AnnotationEndedEvent>().Publish(_currentGT);
        }

        private void BeginAnnotation()
        {
            if (_currentGT != null)
            {
                Confirmation.Raise(new Confirmation(NotificationType.None)
                {
                    Title = "Confirmation",
                    Content = "Do you want to override existing ground truth?"
                }, 
                c => DoAnnotation(c.Confirmed));
            }
            else
            {
                DoAnnotation(true);
            }
        }

        private void DoAnnotation(bool isOverrode)
        {
            _editingChains = isOverrode ? new ObservableCollection<CorefChain>() 
                : new ObservableCollection<CorefChain>(_currentGT);
            _eventAggregator.GetEvent<AnnotationBegunEvent>().Publish(_editingChains);
        }

        private void Unload()
        {
            _emrCollection = null;
            EMRDirPath = null;
            CurrentEMRIndex = -1;

            OnPropertyChanged(nameof(TotalEMRCount));
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
                            Title = "Bad EMR Directory",
                            Content = $"Cannot load. The directory '{d.FileName}' is not a proper EMR directory."
                        });
                    }
                }

                return currentState;
            }
        }

        private void OnReady()
        {
            _emrCollection = new EMRCollection(_emrDirPath);
            CurrentEMRIndex = -1;
            OnPropertyChanged(nameof(TotalEMRCount));
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

            AnnotateCommand.RaiseCanExecuteChanged();
            SaveAnnotationCommand.RaiseCanExecuteChanged();
            CancelAnnotationCommand.RaiseCanExecuteChanged();
        }

        private void OnStateReady()
        {
            UpdateCommandStates();
            OnPropertyChanged(nameof(CurrentState));
        }
    }
}
