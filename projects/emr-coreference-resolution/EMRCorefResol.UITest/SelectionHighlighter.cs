using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace EMRCorefResol.UITest
{
    class SelectionHighlighter : ColorizingTransformer
    {
        private readonly SelectionInfo _info;
        private readonly Brush _bgBrush = null;
        private readonly Brush _fgBrush = null;

        public SelectionHighlighter(SelectionInfo info, Brush bgBrush, Brush fgBrush)
        {
            _info = info;
            _bgBrush = bgBrush;
            _fgBrush = fgBrush;
        }

        protected override void Colorize(ITextRunConstructionContext context)
        {
            if (_info.IsSelected)
            {
                var lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
                var lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;

                if (_info.StartOffset > lineEndOffset || _info.EndOffset < lineStartOffset)
                    return;

                var startCol = _info.StartOffset - lineStartOffset;
                var endCol = _info.EndOffset - lineStartOffset;

                ChangeVisualElements(startCol, endCol, e =>
                {
                    e.TextRunProperties.SetBackgroundBrush(_bgBrush);
                    if (_fgBrush != null)
                        e.TextRunProperties.SetForegroundBrush(_fgBrush);
                });
            }
        }
    }
}
