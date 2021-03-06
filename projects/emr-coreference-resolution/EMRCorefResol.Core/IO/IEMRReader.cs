﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    using Service;
    public interface IEMRReader
    {
        /// <summary>
        /// Reads one <see cref="Concept"/> in one line.
        /// If there are multiple concepts in the line, returns the first one.
        /// </summary>
        /// <param name="line">The line contains the <see cref="Concept"/> to read.</param>
        /// <returns>The <see cref="Concept"/> appears in line.
        /// If there are multiple concepts in the line, returns the first one.
        /// If there are no concepts in the line, returns null.
        /// </returns>
        Concept ReadSingle(string line);

        /// <summary>
        /// Reads multiple <see cref="Concept"/>(s) in one line.
        /// </summary>
        /// <param name="line">The line contains multiple concepts to read.</param>
        /// <returns></returns>
        IEnumerable<Concept> ReadMultiple(string line);

        /// <summary>
        /// Reads <see cref="ConceptType"/> in one line.
        /// </summary>
        /// <param name="line">The line contains the <see cref="ConceptType"/> to read.</param>
        /// <returns></returns>
        ConceptType ReadType(string line);

        /// <summary>
        /// Reads <see cref="EMRSection"/> from EMR content
        /// </summary>
        /// <param name="EMRContent">The whole content of an EMR</param>
        /// <returns></returns>
        List<EMRSection> ReadSection(string EMRContent);


        MedData ReadMedInfoLine(string line);
        Tuple<string, WikiData> ReadWikiFile(string line);
        Tuple<string, UMLSData> ReadUmlsFile(string line);
        Tuple<string, string> ReadTemporalFile(string line);
    }
}
