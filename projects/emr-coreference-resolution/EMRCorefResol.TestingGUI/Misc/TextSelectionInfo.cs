using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    public struct TextSelectionInfo : IEquatable<TextSelectionInfo>
    {
        public static readonly TextSelectionInfo Empty =
            new TextSelectionInfo(string.Empty, -1, -1, -1, -1);

        public string Text { get; }
        public int StartLine { get; }
        public int EndLine { get; }
        public int StartColumn { get; }
        public int EndColumn { get; }

        public TextSelectionInfo(string text, int startLine, 
            int startColumn, int endLine, int endColumn)
            : this()
        {
            Text = text;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }

        public bool Equals(TextSelectionInfo other)
        {
            return Text == other.Text &&
                StartLine == other.StartLine &&
                EndLine == other.EndLine &&
                StartColumn == other.StartColumn &&
                EndColumn == other.EndColumn;
        }

        public override bool Equals(object obj)
        {
            return (obj as TextSelectionInfo?)?.Equals(this) ?? false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator==(TextSelectionInfo s1, TextSelectionInfo s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator!=(TextSelectionInfo s1, TextSelectionInfo s2)
        {
            return !s1.Equals(s2);
        }
    }
}
