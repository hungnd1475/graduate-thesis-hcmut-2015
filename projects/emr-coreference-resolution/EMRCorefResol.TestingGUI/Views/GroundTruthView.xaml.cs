using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for GroundTruthView.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class GroundTruthView
    {
        public static readonly string ID = typeof(GroundTruthView).FullName;

        public GroundTruthView()
            : base(DockableType.Document, ID)
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
