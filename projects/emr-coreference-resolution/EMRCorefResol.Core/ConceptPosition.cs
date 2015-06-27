using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	/// <summary>
	/// Represents a position of concept in an EMR.
	/// </summary>
	public struct ConceptPosition : IComparable, IComparable<ConceptPosition>
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
	}
}
