using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for OutputView.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class OutputView : DockableView
    {
        public static readonly string ID = typeof(OutputView).FullName;

        public OutputView()
            : base(DockableType.Anchorable, ID)
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public OutputView(OutputViewModel vm)
            : this()
        {
            DataContext = vm;
        }
    }
}
