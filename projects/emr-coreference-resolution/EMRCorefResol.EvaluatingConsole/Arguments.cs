using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Core.Console;

namespace HCMUT.EMRCorefResol.EvaluatingConsole
{
    class Arguments
    {
        public string EMRDir { get; set; }
        public string SystemChainsDir { get; set; }
        public string OutputDir { get; set; }
        public string EMRFile { get; set; }
        public EMRFormat Format { get; set; }
        public string AverageFile { get; set; }
    }
}
