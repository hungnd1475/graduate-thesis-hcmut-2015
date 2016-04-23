using HCMUT.EMRCorefResol;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace EMRCorefResol.TestingGUI
{
    public class SelectConceptBehavior : Behavior<TextEditor>
    {
        private SegmentsHighlighter _highlighter;
        private readonly TextSegmentCollection<TextSegment> _selectedSegments
            = new TextSegmentCollection<TextSegment>();

        public Concept SelectedConcept
        {
            get { return (Concept)GetValue(SelectedConceptProperty); }
            set { SetValue(SelectedConceptProperty, value); }
        }
        
        public static readonly DependencyProperty SelectedConceptProperty =
            DependencyProperty.Register(nameof(SelectedConcept), typeof(Concept), typeof(SelectConceptBehavior), 
                new PropertyMetadata(null, SelectedConceptChangedCallback));

        public EMR CurrentEMR
        {
            get { return (EMR)GetValue(CurrentEMRProperty); }
            set { SetValue(CurrentEMRProperty, value); }
        }
        
        public static readonly DependencyProperty CurrentEMRProperty =
            DependencyProperty.Register(nameof(CurrentEMR), typeof(EMR), typeof(SelectConceptBehavior), new PropertyMetadata(null));

        private static void SelectedConceptChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = (SelectConceptBehavior)d;
            var concept = (Concept)e.NewValue;

            var textEditor = b.AssociatedObject;
            var selectedSegments = b._selectedSegments;
            var emr = b.CurrentEMR;

            if (selectedSegments.Count > 0)
            {
                var s = selectedSegments.FirstSegment;
                textEditor.TextArea.TextView.Redraw(s.StartOffset,
                    s.EndOffset - s.StartOffset + 1);
                selectedSegments.Clear();
            }
                        
            if (concept != null)
            {
                var beginIndex = emr.BeginIndexOf(concept);
                var endIndex = emr.EndIndexOf(concept);

                // scroll to the raw text
                textEditor.TextArea.Caret.Offset = beginIndex;
                textEditor.ScrollTo(textEditor.TextArea.Caret.Line, textEditor.TextArea.Caret.Column);

                // store these values for the Highligher to work
                var s = new TextSegment();
                s.StartOffset = beginIndex;
                s.EndOffset = endIndex + 1;
                selectedSegments.Add(s);

                // trigger the redraw process to highlight the raw text
                textEditor.TextArea.TextView.Redraw(beginIndex, endIndex - beginIndex + 1);
            }
        }

        public Brush Foreground { get; set; }
        public Brush Background { get; set; }

        protected override void OnAttached()
        {
            _highlighter = new SegmentsHighlighter(_selectedSegments, Background, Foreground);
            AssociatedObject.TextArea.TextView.LineTransformers.Add(_highlighter);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextArea.TextView.LineTransformers.Remove(_highlighter);
            base.OnDetaching();
        }
    }
}
