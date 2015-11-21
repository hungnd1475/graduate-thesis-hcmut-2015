using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class TemporalData
    {
        public int Start { get; }
        public int End { get; }
        public int Line { get; }
        public string Text { get; }
        public string Value { get; }

        public TemporalData(int start, int end, int line, string text, string value)
        {
            Start = start;
            End = end;
            Line = line;
            Text = text;
            Value = value;
        }

        public TemporalData(int start, int end, string text, string value)
            : this(start, end, 0, text, value)
        {

        }

        public override bool Equals(object obj)
        {
            var tempData = obj as TemporalData;

            if (tempData == null)
            {
                return false;
            }
            else
            {
                return tempData.Start == this.Start &&
                    tempData.End == this.End &&
                    tempData.Line == this.Line &&
                    tempData.Text.Equals(this.Text) &&
                    tempData.Value.Equals(this.Value);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
