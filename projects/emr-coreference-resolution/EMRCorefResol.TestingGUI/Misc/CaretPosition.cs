using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    public struct CaretPosition : IEquatable<CaretPosition>
    {
        public int Line { get; }
        public int Column { get; }

        public CaretPosition(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            return (obj as CaretPosition?)?.Equals(this) ?? false;
        }

        public bool Equals(CaretPosition other)
        {
            return Line == other.Line &&
                Column == other.Column;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
