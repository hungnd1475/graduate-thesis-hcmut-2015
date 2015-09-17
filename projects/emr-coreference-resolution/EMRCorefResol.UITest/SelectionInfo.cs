using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.UITest
{
    class SelectionInfo
    {
        public bool IsSelected { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }

        public SelectionInfo Clone()
        {
            var clone = new SelectionInfo();
            clone.IsSelected = IsSelected;
            clone.StartOffset = StartOffset;
            clone.EndOffset = EndOffset;
            return clone;
        }
    }
}
