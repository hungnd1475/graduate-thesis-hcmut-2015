using HCMUT.EMRCorefResol.Core.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.ResolvingConsole
{
    class Arguments
    {
        public string EMRDirs { get; set; }
        public string EMRName { get; set; }
        public EMRFormat EMRFormat { get; set; }
        public Language Language { get; set; }
        public string ModelsDir { get; set; }
        public ClasMethod ClasMethod { get; set; }
        public ResolMethod ResolMethod { get; set; }
        public string OutputDir { get; set; }
        public string ScoresFile { get; set; }
        public int EMRCount { get; set; }
    }
}
