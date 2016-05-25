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
    /// Interaction logic for EMRCollectionView.xaml
    /// </summary>
    [Export]
    public partial class EMRCollectionView
    {
        public static readonly string ID = typeof(EMRCollectionView).FullName;

        public EMRCollectionView()
            : base(DockableType.Anchorable, ID)
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public EMRCollectionView(EMRCollectionViewModel vm)
            : this()
        {
            DataContext = vm;
        }
    }
}
