using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    class LibSVMScalingFactor
    {
        private readonly double lower, upper;
        private readonly Range<double>[] featureRanges;        
        
        public LibSVMScalingFactor(string restoreFile)
        {            
            using (var sr = new StreamReader(restoreFile))
            {
                sr.ReadLine();
                var s = sr.ReadLine();
                var t = s.Split(' ');
                lower = double.Parse(t[0]);
                upper = double.Parse(t[1]);

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                    t = s.Split(' ');
                    var index = int.Parse(t[0]) - 1;
                    var fMin = double.Parse(t[1]);
                    var fMax = double.Parse(t[2]);
                                        
                }
            }
        }

        public double Scale(int index, double value)
        {
            var fMin = featureRanges[index].Min;
            var fMax = featureRanges[index].Max;

            if (fMin == fMax)
            {
                return 0d;
            }
            else if (value == fMin)
            {
                return lower;
            }
            else if (value == fMax)
            {
                return upper;
            }
            else
            {
                return lower + (upper - lower) * (value - fMin) / (fMax - fMin);
            }
        }


    }
}
