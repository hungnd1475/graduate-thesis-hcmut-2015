using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class EMRSection : IEquatable<EMRSection>
    {
        /// <summary>
        /// Title of an EMR section
        /// Determined by Begin with a Capitol Letter; Ending with : and new line character
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Content of an EMR section
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Index of begining line
        /// </summary>
        public int Begin { get; }

        /// <summary>
        /// Index of ending line
        /// </summary>
        public int End { get; }

        public EMRSection(string title, string content, int begin, int end)
        {
            Title = title;
            Content = content;
            Begin = begin;
            End = end;
        }

        /// <summary>
        /// Checks equality with other <see cref="EMRSection"/> instance.
        /// </summary>
        /// <param name="other">The other <see cref="EMRSection"/> instance.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public bool Equals(EMRSection others)
        {
            return Title.Equals(others.Title, StringComparison.InvariantCultureIgnoreCase) &&
                Begin.Equals(others.Begin) &&
                End.Equals(others.End);
        }

        public override bool Equals(object obj)
        {
            var c = obj as EMRSection;
            return (c != null) ? Equals(c) : false;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Title, Begin, End);
        }

        public override string ToString()
        {
            return "Section: " + Title + "| Begin line: " + Begin + "| End line: " + End;
        }
    }
}
