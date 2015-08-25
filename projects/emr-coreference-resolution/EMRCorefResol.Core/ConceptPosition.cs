using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents a position of concept in an EMR.
    /// </summary>
    public struct ConceptPosition : IComparable, IComparable<ConceptPosition>, IEquatable<ConceptPosition>
    {
        /// <summary>
        /// The line where concept appears.
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// The letter index.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Initializes a <see cref="ConceptPosition"/> instance.
        /// </summary>
        /// <param name="line">The line number.</param>
        /// <param name="column">The letter index.</param>
        public ConceptPosition(int line, int column)
            : this()
        {
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Compares to another <see cref="ConceptPosition"/> instance.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ConceptPosition other)
        {
            if (this.Line == other.Line)
            {
                return this.Column - other.Column;
            }
            else
            {
                return this.Line - other.Line;
            }
        }

        public int CompareTo(object obj)
        {
            var position = obj as ConceptPosition?;
            if (position != null)
                return CompareTo(position.Value);
            throw new ArgumentNullException();
        }

        public override string ToString()
        {
            return $"{Line}:{Column}";
        }

        public static ConceptPosition Parse(string s)
        {
            var p = s.Split(':');
            var line = int.Parse(p[0]);
            var col = int.Parse(p[1]);
            return new ConceptPosition(line, col);
        }

        public bool Equals(ConceptPosition other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            var pos = obj as ConceptPosition?;
            return (pos == null) ? false : CompareTo(pos.Value) == 0;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Line, Column);
        }
    }
}
