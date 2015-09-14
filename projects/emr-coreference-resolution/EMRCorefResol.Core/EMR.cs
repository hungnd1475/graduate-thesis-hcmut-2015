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

        public string getLine(int line)
        {
            var lines = Content.Split('\n');

            if(line > lines.Count() - 1)
            {
                return null;
            }
            return lines[line-1];
        }
    }
}
