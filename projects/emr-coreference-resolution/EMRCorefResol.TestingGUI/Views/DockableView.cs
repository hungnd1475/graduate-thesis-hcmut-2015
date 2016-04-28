using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    public abstract class DockableView : UserControl, IDockableView
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(DockableView), new PropertyMetadata(null));

        public DockableType DockableType { get; }
        public string ContentId { get; }

        protected DockableView(DockableType type, string id)
        {
            DockableType = type;
            ContentId = id;
        }
    }
}
