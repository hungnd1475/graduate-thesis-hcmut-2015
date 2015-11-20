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

        public bool SearchBestRegion { get; }
        public double BestRegionRadius { get; }
        public double BestRegionStep { get; } 

        public GridSearchConfig(Range<double> costRange, double costStep, 
            Range<double> gammaRange, double gammaStep, bool searchBestRegion)
            : this(costRange, costStep, gammaRange, gammaStep, searchBestRegion, 2, 0.25)
        {
        }

        public GridSearchConfig(Range<double> costRange, double costStep,
            Range<double> gammaRange, double gammaStep, bool searchBestRegion,
            double bestRegionRadius, double bestRegionStep)
        {
            CostRange = costRange;
            CostStep = costStep;

            GammaRange = gammaRange;
            GammaStep = gammaStep;

            SearchBestRegion = searchBestRegion;

            BestRegionRadius = bestRegionRadius;
            BestRegionStep = bestRegionStep;
        }
    }
}
