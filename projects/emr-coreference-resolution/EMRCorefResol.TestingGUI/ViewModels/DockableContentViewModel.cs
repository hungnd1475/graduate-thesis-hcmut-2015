using Prism.Mvvm;

namespace EMRCorefResol.TestingGUI
{
    public abstract class DockableContentViewModel : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string ContentId { get; }

        public DockableContentViewModel(string id)
        {
            ContentId = id;
        }
    }
}
