using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace EMRCorefResol.TestingGUI
{
    class SegmentsHighlighter : ColorizingTransformer
    {
        private readonly TextSegmentCollection<TextSegment> _focusedSegments;
        private readonly Brush _background, _foreground;

        public SegmentsHighlighter(TextSegmentCollection<TextSegment> focusedSegments,
            Brush background, Brush foreground)
        {
            _focusedSegments = focusedSegments;
            _background = background;
            _foreground = foreground;
        }

        protected override void Colorize(ITextRunConstructionContext context)
        {
            var lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
            var lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;

            foreach (var segment in _focusedSegments.FindOverlappingSegments(lineStartOffset, lineEndOffset - lineStartOffset + 1))
            {
                var startCol = segment.StartOffset - lineStartOffset;
                var endCol = segment.EndOffset - lineStartOffset;

                ChangeVisualElements(startCol, endCol, e =>
                {
                    e.TextRunProperties.SetBackgroundBrush(_background);
                    if (_foreground != null)
                        e.TextRunProperties.SetForegroundBrush(_foreground);
                });
            }
        }
    }
}
