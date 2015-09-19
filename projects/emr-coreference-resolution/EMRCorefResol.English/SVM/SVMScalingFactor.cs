using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.SVM
{
    class SVMScalingFactor
    {
        public class Range
        {
            public double Lower { get; set; }
            public double Upper { get; set; }

            public Range(double lower, double upper)
            {
                Lower = lower;
                Upper = upper;
            }
        }

        public Range NewRange { get; }
        public Range[] FeatureRanges { get; }

        public SVMScalingFactor(Range newRange, Range[] featureRanges)
        {
            NewRange = newRange;
            FeatureRanges = featureRanges;
        }
    }
}
