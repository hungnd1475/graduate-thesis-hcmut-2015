using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Windows;

namespace EMRCorefResol.TestingGUI
{
    public class BindableTextEditor : TextEditor
    {
        private SearchPanel _searchPanel;

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
            DependencyProperty.Register(nameof(ScrollToTopWhenTextChanged), typeof(bool), typeof(BindableTextEditor), 
                new PropertyMetadata(false));

        public TextSelectionInfo Selection
        {
            get { return (TextSelectionInfo)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }
        
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register(nameof(Selection), typeof(TextSelectionInfo), typeof(BindableTextEditor), 
                new PropertyMetadata(TextSelectionInfo.Empty));

        public bool IsSearchEnabled
        {
            get { return (bool)GetValue(IsSearchEnabledProperty); }
            set { SetValue(IsSearchEnabledProperty, value); }
        }
        
        public static readonly DependencyProperty IsSearchEnabledProperty =
            DependencyProperty.Register(nameof(IsSearchEnabled), typeof(bool), typeof(BindableTextEditor), 
                new PropertyMetadata(false, IsSearchEnabledChangeCallback));

        private static void IsSearchEnabledChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (BindableTextEditor)d;
            var isSearchEnabled = (bool)e.NewValue;
            if (isSearchEnabled)
            {
                editor._searchPanel = SearchPanel.Install(editor);
            }
            else
            {
                editor._searchPanel?.Uninstall();
            }
        }

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
            TextArea.SelectionChanged += TextArea_SelectionChanged;
        }
            
        private void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            var text = TextArea.Selection.GetText();
            var startLine = TextArea.Selection.StartPosition.Line;
            var startCol = TextArea.Selection.StartPosition.Column;
            var endLine = TextArea.Selection.EndPosition.Line;
            var endCol = TextArea.Selection.EndPosition.Column;
            Selection = new TextSelectionInfo(text, startLine, startCol, endLine, endCol);
        }
    }
}
