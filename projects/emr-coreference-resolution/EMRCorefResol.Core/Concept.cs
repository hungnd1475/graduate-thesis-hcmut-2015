﻿using HCMUT.EMRCorefResol.Utilities;
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
    public class Concept : IEquatable<Concept>
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
        public ConceptType Type { get; private set; }

        /// <summary>
        /// Initializes a <see cref="Concept"/> instance.
        /// </summary>
        /// <param name="lexicon">The string.</param>
        /// <param name="begin">The beginning position.</param>
        /// <param name="end">The ending position.</param>
        /// <param name="type">The concept type.</param>
        public Concept(string lexicon, ConceptPosition begin, ConceptPosition end, ConceptType type)
        {
            this.Lexicon = lexicon;
            this.Begin = begin;
            this.End = end;
            this.Type = type;
        }

        public bool Equals(Concept other)
        {
            return string.Equals(Lexicon, other.Lexicon) &&
                other.Begin.Equals(Begin) &&
                other.End.Equals(End) &&
                other.Type == Type;
        }

        public override bool Equals(object obj)
        {
            var c = obj as Concept;
            return (c != null) ? Equals(c) : false;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(new object[] { Lexicon, Begin, End, Type });
        }

        public override string ToString()
        {
            return $"c=\"{Lexicon}\" {Begin}:{End}||t=\"{Type.ToString().ToLower()}\"";
        }

        public string ToString(bool includeType)
        {
            return includeType ? ToString() : $"c=\"{Lexicon}\" {Begin}:{End}";
        }
    }
}
