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

        public ScrollDirection ScrollWhenTextChanged
        {
            get { return (ScrollDirection)GetValue(ScrollWhenTextChangedProperty); }
            set { SetValue(ScrollWhenTextChangedProperty, value); }
        }

        public static readonly DependencyProperty ScrollWhenTextChangedProperty =
            DependencyProperty.Register(nameof(ScrollWhenTextChanged), typeof(ScrollDirection), typeof(BindableTextEditor),
                new PropertyMetadata(ScrollDirection.None));

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

        public CaretPosition CaretPosition
        {
            get { return (CaretPosition)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
        }
        
        public static readonly DependencyProperty CaretPositionProperty =
            DependencyProperty.Register("CaretPosition", typeof(CaretPosition), typeof(BindableTextEditor), 
                new PropertyMetadata(new CaretPosition()));
        
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

            switch (editor.ScrollWhenTextChanged)
            {
                case ScrollDirection.ToTop:
                    editor.TextArea.Caret.Offset = 0;
                    editor.TextArea.Caret.BringCaretToView();
                    break;
                case ScrollDirection.ToBottom:
                    editor.TextArea.Caret.Column = 0;
                    editor.TextArea.Caret.Line = editor.Document.LineCount - 1;
                    editor.TextArea.Caret.BringCaretToView();
                    break;
            }
        }

        public BindableTextEditor()
        {
            TextArea.SelectionCornerRadius = 0;
            TextArea.SelectionBorder = null;
            TextArea.SelectionChanged += TextArea_SelectionChanged;
            TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            var line = TextArea.Caret.Line;
            var column = TextArea.Caret.Column;
            CaretPosition = new CaretPosition(line, column);
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
