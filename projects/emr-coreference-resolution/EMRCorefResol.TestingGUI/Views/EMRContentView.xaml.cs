using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for EMRContentView.xaml
    /// </summary>
    [Export]
    public partial class EMRContentView : UserControl, IDecideDockableType
    {
        public EMRContentView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public EMRContentView(EMRContentViewModel emrContentVM)
            :this()
        {
            DataContext = emrContentVM;
        }

        public DockableType DockableType
        {
            get { return DockableType.Document; }
        }
    }
}
