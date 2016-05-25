using HCMUT.EMRCorefResol;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;
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

        public IReadOnlyList<Concept> SelectedConcepts
        {
            get { return (IReadOnlyList<Concept>)GetValue(SelectedConceptsProperty); }
            set { SetValue(SelectedConceptsProperty, value); }
        }
        
        public static readonly DependencyProperty SelectedConceptsProperty =
            DependencyProperty.Register(nameof(SelectedConcepts), typeof(IReadOnlyList<Concept>), 
                typeof(SelectConceptBehavior), 
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
            var selectedConcepts = (IReadOnlyList<Concept>)e.NewValue;

            var textEditor = b.AssociatedObject;
            var selectedSegments = b._selectedSegments;
            var emr = b.CurrentEMR;

            if (selectedSegments.Count > 0)
            {
                var startOffset = selectedSegments.FirstSegment.StartOffset;
                var endOffset = selectedSegments.LastSegment.EndOffset;

                selectedSegments.Clear();
                textEditor.TextArea.TextView.Redraw(startOffset,
                    endOffset - startOffset + 1);
            }
                        
            if (selectedConcepts != null && selectedConcepts.Count > 0)
            {
                foreach (var c in selectedConcepts)
                {
                    var beginIndex = emr.BeginIndexOf(c);
                    var endIndex = emr.EndIndexOf(c);

                    // scroll to the raw text
                    textEditor.TextArea.Caret.Offset = beginIndex;
                    textEditor.TextArea.Caret.BringCaretToView();

                    // store these values for the Highligher to work
                    var s = new TextSegment();
                    s.StartOffset = beginIndex;
                    s.EndOffset = endIndex + 1;
                    selectedSegments.Add(s);
                }

                var startOffset = selectedSegments.FirstSegment.StartOffset;
                var endOffset = selectedSegments.LastSegment.EndOffset;
                // trigger the redraw process to highlight the raw text
                textEditor.TextArea.TextView.Redraw(startOffset, endOffset - startOffset + 1);
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
