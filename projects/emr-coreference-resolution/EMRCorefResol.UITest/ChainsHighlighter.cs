using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit;

namespace EMRCorefResol.UITest
{
    class ChainsHighlighter : IBackgroundRenderer
    {
        public ChainsInfo ChainsInfo { get; } = new ChainsInfo();

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (!ChainsInfo.CurrentLine.HasValue)
                return;

            if (!textView.VisualLinesValid)
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            int viewStart = visualLines[0].FirstDocumentLine.Offset;
            int viewEnd = visualLines[visualLines.Count - 1].LastDocumentLine.Offset;

            foreach (var segment in ChainsInfo.Segments.FindOverlappingSegments(viewStart, viewEnd - viewStart + 1))
            {
                var geoBuilder = new BackgroundGeometryBuilder();
                geoBuilder.AlignToMiddleOfPixels = true;
                geoBuilder.AddSegment(textView, segment);

                var geometry = geoBuilder.CreateGeometry();
                if (geometry != null)
                {
                    drawingContext.DrawGeometry(new SolidColorBrush(Color.FromRgb(250, 170, 137)), null, geometry);
                }
            }
        }
    }
}
