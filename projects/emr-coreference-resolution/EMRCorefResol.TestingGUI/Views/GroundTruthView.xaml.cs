using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for GroundTruthView.xaml
    /// </summary>
    [Export]
    public partial class GroundTruthView : UserControl
    {
        public GroundTruthView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public GroundTruthView(GroundTruthViewModel gtVM)
            : this()
        {
            this.DataContext = gtVM;
        }
    }
}
