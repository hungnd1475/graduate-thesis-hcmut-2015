using HCMUT.EMRCorefResol;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace EMRCorefResol.UITest
{
    /// <summary>
    /// Interaction logic for ConceptTypeChooser.xaml
    /// </summary>
    public partial class ConceptTypeChooser : Window
    {
        public ConceptType SelectedType
        {
            get
            {
                return (ConceptType)Enum.Parse(typeof(ConceptType), (string)lbTypes.SelectedValue);
            }
        }

        public ConceptTypeChooser()
        {
            InitializeComponent();
            lbTypes.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
