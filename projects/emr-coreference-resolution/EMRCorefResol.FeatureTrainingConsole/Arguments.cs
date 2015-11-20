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
        
        public double Cost { get; set; } // for LibSVM only
        public double Gamma { get; set; } // for LibSVM only
        public int ApplyWeights { get; set; } // for LibSVM only
        public int ShouldLog { get; set; } // for LibSVM only
        public int CacheSize { get; set; } // for LibSVM only
    }
}
