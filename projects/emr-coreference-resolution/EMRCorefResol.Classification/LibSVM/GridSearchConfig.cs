using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    /// <summary>
    /// Parameters config for RBF kernel grid-search.
    /// </summary>
    public class GridSearchConfig
    {
        public Range<double> CostRange { get; }
        public double CostStep { get; }

        public Range<double> GammaRange { get; }
        public double GammaStep { get; }

        public GridSearchConfig(Range<double> costRange, double costStep, 
            Range<double> gammaRange, double gammaStep)
        {
            CostRange = costRange;
            CostStep = costStep;

            GammaRange = gammaRange;
            GammaStep = gammaStep;
        }
    }
}
