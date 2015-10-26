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
        public string GroundTruthDir { get; set; }
        public string SystemChainsDir { get; set; }
        public string OutputFile { get; set; }
        public string ChainsFile { get; set; }
        public EMRFormat Format { get; set; }
    }
}
