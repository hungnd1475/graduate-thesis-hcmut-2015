using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents a concept in an EMR.
    /// </summary>
    public class Concept : IEquatable<Concept>, IComparable<Concept>, IComparable
    {
        /// <summary>
        /// The string representing the concept.
        /// </summary>
        public string Lexicon { get; }
        /// <summary>
        /// The beginning position of the concept.
        /// </summary>
        public ConceptPosition Begin { get; }
        /// <summary>
        /// The ending position of the concept.
        /// </summary>
        public ConceptPosition End { get; }
        /// <summary>
        /// The type of concept.
        /// </summary>
        public ConceptType Type { get; }

        /// <summary>
        /// Initializes a <see cref="Concept"/> instance.
        /// </summary>
        /// <param name="lexicon">The string represents the concept.</param>
        /// <param name="begin">The begin position of the concept.</param>
        /// <param name="end">The end position of the concept.</param>
        /// <param name="type">The concept type.</param>
        public Concept(string lexicon, ConceptPosition begin, ConceptPosition end, ConceptType type)
        {
            this.Lexicon = lexicon;
            this.Begin = begin;
            this.End = end;
            this.Type = type;
        }

        /// <summary>
        /// Initializes a <see cref="Concept"/> instance, with type set to <see cref="ConceptType.None"/>.
        /// </summary>
        /// <param name="lexicon">The string represents the concept.</param>
        /// <param name="begin">The begin position of the concept.</param>
        /// <param name="end">The end position of the concept.</param>
        public Concept(string lexicon, ConceptPosition begin, ConceptPosition end)
            : this(lexicon, begin, end, ConceptType.None)
        { }

        /// <summary>
        /// Checks equality with other <see cref="Concept"/> instance.
        /// </summary>
        /// <param name="other">The other <see cref="Concept"/> instance.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public bool Equals(Concept other)
        {
            return string.Equals(Lexicon, other.Lexicon) &&
                other.Begin.Equals(Begin) &&
                other.End.Equals(End);
        }

        public override bool Equals(object obj)
        {
            var c = obj as Concept;
            return (c != null) ? Equals(c) : false;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Lexicon, Begin, End);
        }

        public override string ToString()
        {
            return $"c=\"{Lexicon}\" {Begin} {End}";
        }

        public int CompareTo(Concept other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            var compareBegin = Begin.CompareTo(other.Begin);
            return compareBegin != 0 ? compareBegin : End.CompareTo(other.End);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Concept);
        }

        public Concept Clone()
        {
            return Clone(Type);
        }

        public Concept Clone(ConceptType conceptType)
        {
            return new Concept(Lexicon, Begin, End, conceptType);
        }
    }
}
