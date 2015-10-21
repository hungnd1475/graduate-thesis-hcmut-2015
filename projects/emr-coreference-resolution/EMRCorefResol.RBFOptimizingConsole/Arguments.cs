using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.RBFOptimizingConsole
{
    class Arguments
    {
        public string DataPath { get; set; }
        public int NFold { get; set; }
        public string GammaRange { get; set; }
        public string CostRange { get; set; }
        public int SearchBestRegion { get; set; }
        public string BestRegion { get; set; }
        public string LogFile { get; set; }
    }
}
