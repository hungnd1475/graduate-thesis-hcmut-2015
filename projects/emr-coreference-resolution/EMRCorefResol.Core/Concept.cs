using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	/// <summary>
	/// Represents a concept in an EMR.
	/// </summary>
    public class Concept
    {
		/// <summary>
		/// The string representing the concept.
		/// </summary>
		public string Lexicon { get; private set; }
		/// <summary>
		/// The beginning position of the concept.
		/// </summary>
		public ConceptPosition Begin { get; private set; }
		/// <summary>
		/// The ending position of the concept.
		/// </summary>
		public ConceptPosition End { get; private set; }
		/// <summary>
		/// The type of concept.
		/// </summary>
		public EntityType Type { get; private set; }

		/// <summary>
		/// Initializes a <see cref="Concept"/> instance.
		/// </summary>
		/// <param name="lexicon">The string.</param>
		/// <param name="begin">The beginning position.</param>
		/// <param name="end">The ending position.</param>
		/// <param name="type">The concept type.</param>
		public Concept(string lexicon, ConceptPosition begin, ConceptPosition end, EntityType type)
		{
			this.Lexicon = lexicon;
			this.Begin = begin;
			this.End = end;
			this.Type = type;
		}
    }
}
