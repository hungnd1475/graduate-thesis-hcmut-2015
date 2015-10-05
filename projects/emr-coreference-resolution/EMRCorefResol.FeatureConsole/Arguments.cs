using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.FeatureConsole
{
    class Arguments
    {
        public List<string> EMRPaths { get; set; }
        public Mode Mode { get; set; }
        public Language Language { get; set; }
        public string OutDir { get; set; }
        public DataFormat DataFormat { get; set; }
        public OutputFormat OutputFormat { get; set; }
        public string ModelPath { get; set; }
        public int Random { get; set; }
    }
}
