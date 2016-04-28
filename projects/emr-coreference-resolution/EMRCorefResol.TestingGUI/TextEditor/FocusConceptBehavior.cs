using HCMUT.EMRCorefResol;
using HCMUT.EMRCorefResol.IO;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace EMRCorefResol.TestingGUI
{
    public class FocusConceptBehavior : Behavior<TextEditor>
    {
        private static Regex ConceptPattern = new Regex("c=\"[^|]+\" \\d+:\\d+ \\d+:\\d+");
        private readonly TextSegmentCollection<TextSegment> _focusedSegments =
            new TextSegmentCollection<TextSegment>();
        private readonly List<Concept> _focusedConcepts = new List<Concept>();

        private readonly IEMRReader emrReader = new I2B2EMRReader();
        private SegmentsHighlighter _highlighter;

        public IReadOnlyList<Concept> FocusedConcepts
        {
            get { return (IReadOnlyList<Concept>)GetValue(FocusedConceptsProperty); }
            set { SetValue(FocusedConceptsProperty, value); }
        }

        public static readonly DependencyProperty FocusedConceptsProperty =
            DependencyProperty.Register(nameof(FocusedConcepts), typeof(IReadOnlyList<Concept>), typeof(FocusConceptBehavior),
                new PropertyMetadata(null));

        public Brush Foreground { get; set; }

        public Brush Background { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;

            _highlighter = new SegmentsHighlighter(_focusedSegments, Background, Foreground);
            AssociatedObject.TextArea.TextView.LineTransformers.Add(_highlighter);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
            AssociatedObject.TextArea.TextView.LineTransformers.Remove(_highlighter);
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            base.OnDetaching();
        }

        private void AssociatedObject_TextChanged(object sender, EventArgs e)
        {
            if (sender == AssociatedObject)
            {
                ClearFocusedIfAny();
                FocusedConcepts = _focusedConcepts.AsReadOnly();
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            var txt = AssociatedObject;
            var caretOffset = txt.CaretOffset; // first get the caret offset

            if (_focusedSegments.FindSegmentsContaining(caretOffset).Count > 0)
            {
                // if the caret lies on the already tracked focused segments, do nothing
                return;
            }

            var caretCol = txt.TextArea.Caret.Location.Column - 1;
            var line = txt.Document.GetLineByOffset(caretOffset); // get the line info the caret currently lies on
            var lineText = txt.Document.GetText(line.Offset, line.Length); // retrieve the line text

            var startAt = caretCol == lineText.Length ? caretCol - 1 : caretCol; // we will start at the caret column
            var focusedText = string.Empty; // stores the focused text (if any)

            if (!string.IsNullOrEmpty(lineText) && startAt >= 0 && startAt < lineText.Length)
            {
                // we will travel from the current caret column to the starting index of the current line (i.e. 0)
                // each time we go back, we check if there is a match with the concept pattern start at our column
                // if there is, check if the caret lies within the match value, if it is store the value and stop, 
                // otherwise go back one char and repeat the above process until we reach the start of the line.
                while (true)
                {
                    var match = ConceptPattern.Match(lineText, startAt);

                    if (match.Success)
                    {
                        var text = match.Value;
                        var beginIndex = lineText.IndexOf(text);
                        var endIndex = beginIndex + text.Length;
                        if (beginIndex <= caretCol && endIndex >= caretCol)
                        {
                            focusedText = text;
                            break;
                        }
                    }

                    if (startAt > 0)
                        startAt -= 1;
                    else break;
                }
            }

            Concept currentFocusedConcept = null;
            TextSegment currentFocusedSegment = null;

            if (!string.IsNullOrEmpty(focusedText))
            {
                // if there is a focused text, parse it to a Concept instance
                currentFocusedConcept = emrReader.ReadSingle(focusedText);

                // store the text position
                currentFocusedSegment = new TextSegment();
                currentFocusedSegment.StartOffset = line.Offset + startAt;
                currentFocusedSegment.Length = focusedText.Length;
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // if user don't want to focus multiple concepts using Ctrl key,
                // remove all currently tracked segments and coresponding concepts
                ClearFocusedIfAny();
            }

            if (currentFocusedSegment != null && currentFocusedConcept != null)
            {
                // add the current focused segment if it presents
                _focusedSegments.Add(currentFocusedSegment);
                txt.TextArea.TextView.Redraw(currentFocusedSegment.StartOffset, currentFocusedSegment.Length);

                // also track the coresponding concept
                _focusedConcepts.Add(currentFocusedConcept);
            }

            FocusedConcepts = _focusedConcepts.AsReadOnly();
        }

        private void ClearFocusedIfAny()
        {
            if (_focusedSegments.Count > 0)
            {
                var startOffset = _focusedSegments.FirstSegment.StartOffset;
                var endOffset = _focusedSegments.LastSegment.EndOffset;
                _focusedSegments.Clear();
                AssociatedObject.TextArea.TextView.Redraw(startOffset, endOffset - startOffset + 1);
                _focusedConcepts.Clear();
            }
        }
    }
}
