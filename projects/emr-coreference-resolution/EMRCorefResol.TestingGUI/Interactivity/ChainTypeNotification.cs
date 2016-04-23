using HCMUT.EMRCorefResol;

namespace EMRCorefResol.TestingGUI
{
    public class ChainTypeNotification : Notification
    {
        private ConceptType _selectedType;
        public ConceptType SelectedType
        {
            get { return _selectedType; }
            set { SetProperty(ref _selectedType, value); }
        }

        public ChainTypeNotification()
            : base(NotificationType.None)
        {
        }
    }
}
