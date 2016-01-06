using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.UITest
{
    class ChainsInfo
    {
        public TextSegmentCollection<TextSegment> Segments { get; set; }
        public int? CurrentLine { get; set; }    
        public object Raiser { get; set; }
        public bool Active { get; set; }
    }
}
