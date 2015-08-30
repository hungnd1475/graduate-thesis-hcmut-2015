using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Represents an EMR with entities recognized.
    /// </summary>
    public class EMR
    {
        /// <summary>
        /// Gets the raw EMR content.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the collection of concepts recognized from EMR.
        /// </summary>
        public ConceptCollection Concepts { get; }

        /// <summary>
        /// Initializes an <see cref="EMR"/> instance from raw content and concepts file.
        /// </summary>
        /// <param name="emrFile">The path to the content file.</param>
        /// <param name="conceptsFile">The path to the concepts file.</param>
        /// <param name="dataReader">The reader that can read the concepts from concepts file.</param>
        public EMR(string emrFile, string conceptsFile, IDataReader dataReader)
        {
            var fs = new FileStream(emrFile, FileMode.Open);
            var sr = new StreamReader(fs);
            Content = sr.ReadToEnd();
            sr.Close();

            Concepts = new ConceptCollection(conceptsFile, dataReader);            
        }

        /// <summary>
        /// Calculates the first letter index of a specified concept in the raw content.
        /// </summary>
        /// <param name="c">The concept.</param>
        /// <returns></returns>
        public int BeginIndexOf(Concept c)
        {
            int line = 1, index = 0, nextIndex = 0;
            while (line < c.Begin.Line)
            {
                index = nextIndex;
                nextIndex = Content.IndexOf(Environment.NewLine, nextIndex) + Environment.NewLine.Length;
                line += 1;
            }

            int word = -1;
            while (word < c.Begin.WordIndex)
            {
                index = nextIndex;
                nextIndex = Content.IndexOf(' ', nextIndex) + 1;
                word += 1;
            }

            return index;
        }

        /// <summary>
        /// Calculates the last letter index of a specified concept in the raw content.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int EndIndexOf(Concept c)
        {
            var bIndex = BeginIndexOf(c);
            return bIndex + c.Lexicon.Length - 1;
        }
    }
}
