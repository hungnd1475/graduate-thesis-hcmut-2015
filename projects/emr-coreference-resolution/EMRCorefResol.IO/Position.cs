using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public struct Position : IComparable, IComparable<Position>
	{
		public int Line { get; private set; }
		public int Column { get; private set; }

		public Position(int line, int column)
			: this()
		{
			this.Line = line;
			this.Column = column;
		}

		public int CompareTo(Position other)
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
			var position = obj as Position?;
			if (position != null)
				return CompareTo(position.Value);
			throw new ArgumentNullException();
		}
	}
}
