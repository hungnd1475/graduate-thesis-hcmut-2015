using HCMUT.EMRCorefResol;
using Prism.Events;
using Prism.Regions;
using System.ComponentModel.Composition;

namespace EMRCorefResol.TestingGUI
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EMRContentViewModel : DockableContentViewModel
    {
        public const string ID = "EMRContent";

        private readonly IRegionManager _regionManager;

        private EMR _currentEMR;
        public EMR CurrentEMR
        {
            get { return _currentEMR; }
            set { SetProperty(ref _currentEMR, value); }
        }

        private Concept _selectedConcept;
        public Concept SelectedConcept
        {
            get { return _selectedConcept; }
            set { SetProperty(ref _selectedConcept, value); }
        }

        [ImportingConstructor]
        public EMRContentViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
            : base(ID)
        {
            Title = "EMR";
            CurrentEMR = null;

            eventAggregator.GetEvent<EMRChangedEvent>().Subscribe(EMRChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<SelectedConceptChangedEvent>().Subscribe(SelectedConceptChanged, ThreadOption.UIThread);
            eventAggregator.GetEvent<EMRBaseChangedEvent>().Subscribe(OnEMRBaseChanged, ThreadOption.UIThread);

            _regionManager = regionManager;
        }

        private void OnEMRBaseChanged(bool zeroBase)
        {
            if (_selectedConcept != null)
            {
                var t = _selectedConcept;
                SelectedConcept = null;
                SelectedConcept = t;
            }
        }

        private void SelectedConceptChanged(Concept selectedConcept)
        {
            SelectedConcept = selectedConcept;
        }

        private void EMRChanged(EMRChangedEventArgs e)
        {
            CurrentEMR = e?.EMR;
            Title = e != null ? $"EMR: {e.EMR.GetEMRName()}" : $"EMR";
        }
    }
}
