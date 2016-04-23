using System.Windows;
using System.Windows.Controls;

namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class NotificationContent : UserControl
    {
        public DataTemplate NotificationTemplate
        {
            get { return (DataTemplate)GetValue(NotificationTemplateProperty); }
            set { SetValue(NotificationTemplateProperty, value); }
        }
        
        public static readonly DependencyProperty NotificationTemplateProperty =
            DependencyProperty.Register("NotificationTemplate", typeof(DataTemplate), typeof(NotificationContent), new PropertyMetadata(null));

        public DataTemplate ButtonTemplate
        {
            get { return (DataTemplate)GetValue(ButtonTemplateProperty); }
            set { SetValue(ButtonTemplateProperty, value); }
        }
        
        public static readonly DependencyProperty ButtonTemplateProperty =
            DependencyProperty.Register("ButtonTemplate", typeof(DataTemplate), typeof(NotificationContent), new PropertyMetadata(null));

        public NotificationContent()
        {
            InitializeComponent();
        }
    }
}
