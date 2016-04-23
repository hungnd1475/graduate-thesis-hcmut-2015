using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for EMRConceptsView.xaml
    /// </summary>
    [Export]
    public partial class EMRConceptsView : UserControl
    {
        public EMRConceptsView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public EMRConceptsView(EMRConceptsViewModel emrConceptsVM)
            : this()
        {
            DataContext = emrConceptsVM;
        }
    }
}
