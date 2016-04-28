using HCMUT.EMRCorefResol;

namespace EMRCorefResol.TestingGUI
{
    public class ConceptTypeNotification : Notification
    {
        private ConceptType _selectedType;
        public ConceptType SelectedType
        {
            get { return _selectedType; }
            set { SetProperty(ref _selectedType, value); }
        }

        public ConceptTypeNotification()
            : base(NotificationType.None)
        {
        }
    }
}
