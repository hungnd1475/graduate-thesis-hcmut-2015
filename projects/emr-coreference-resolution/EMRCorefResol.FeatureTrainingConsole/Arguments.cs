using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Core.Console;

namespace HCMUT.EMRCorefResol.FeatureTrainingConsole
{
    class Arguments
    {
        public string FeaturePath { get; set; }
        public ClasMethod ClasMethod { get; set; }
        public Instance Instance { get; set; }
        public string OutputDir { get; set; }

        public string CostRange { get; set; } // for LibSVM only
        public string GammaRange { get; set; } // for LibSVM only
        public bool SearchBestRegion { get; set; }
    }
}
