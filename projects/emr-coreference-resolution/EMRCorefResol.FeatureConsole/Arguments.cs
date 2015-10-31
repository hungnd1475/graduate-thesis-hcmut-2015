using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Core.Console;

namespace HCMUT.EMRCorefResol.FeatureConsole
{
    class Arguments
    {
        public string EMRDir { get; set; }
        public Mode Mode { get; set; }
        public Language Language { get; set; }
        public string OutputDir { get; set; }
        public EMRFormat EMRFormat { get; set; }
        public ClasMethod ClasMethod { get; set; }
        public string ModelsDir { get; set; }
        public int Random { get; set; }
        public int DoScale { get; set; }
        public string Instances { get; set; }
        public string EMRFile { get; set; }
    }
}
