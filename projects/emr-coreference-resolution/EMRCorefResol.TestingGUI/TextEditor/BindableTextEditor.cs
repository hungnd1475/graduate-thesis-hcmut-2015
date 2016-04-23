using ICSharpCode.AvalonEdit;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    public class BindableTextEditor : TextEditor
    {
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BindableTextEditor), 
                new FrameworkPropertyMetadata(TextChangedCallback));

        public bool ScrollToTopWhenTextChanged
        {
            get { return (bool)GetValue(ScrollToTopWhenTextChangedProperty); }
            set { SetValue(ScrollToTopWhenTextChangedProperty, value); }
        }
        
        public static readonly DependencyProperty ScrollToTopWhenTextChangedProperty =
            DependencyProperty.Register(nameof(ScrollToTopWhenTextChanged), typeof(bool), typeof(BindableTextEditor), new PropertyMetadata(false));

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (BindableTextEditor)d;
            var text = (string)e.NewValue;
            editor.Document.Text = text;
            
            if (editor.ScrollToTopWhenTextChanged)
            {
                editor.TextArea.Caret.Offset = 0;
                editor.ScrollTo(0, 0);
            }
        }

        public BindableTextEditor()
        {
            TextArea.SelectionCornerRadius = 0;               
        }
    }
}
