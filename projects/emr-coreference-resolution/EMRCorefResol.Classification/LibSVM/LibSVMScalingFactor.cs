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
        private readonly double _lower, _upper;
        private readonly Dictionary<int, Range<double>> _featureRanges;

        private LibSVMScalingFactor(double lower, double upper,
            Dictionary<int, Range<double>> featureRanges)
        {
            _lower = lower;
            _upper = upper;
            _featureRanges = featureRanges;
        }

        public double Scale(int index, double value)
        {
            if (_featureRanges.ContainsKey(index))
            {
                var fMin = _featureRanges[index].Min;
                var fMax = _featureRanges[index].Max;

                if (fMin == fMax)
                {
                    return 0d;
                }
                else if (value == fMin)
                {
                    return _lower;
                }
                else if (value == fMax)
                {
                    return _upper;
                }
                else
                {
                    return _lower + (_upper - _lower) * (value - fMin) / (fMax - fMin);
                }
            }

            return value;
        }

        public static LibSVMScalingFactor Load(string restoreFile)
        {
            if (string.IsNullOrWhiteSpace(restoreFile) || !File.Exists(restoreFile))
            {
                return null;
            }

            using (var sr = new StreamReader(restoreFile))
            {
                sr.ReadLine();
                var s = sr.ReadLine();
                var t = s.Split(' ');
                var lower = double.Parse(t[0]);
                var upper = double.Parse(t[1]);

                var featureRanges = new Dictionary<int, Range<double>>();

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                    t = s.Split(' ');
                    var index = int.Parse(t[0]) - 1;
                    var fMin = double.Parse(t[1]);
                    var fMax = double.Parse(t[2]);
                    featureRanges.Add(index, Range.Create(fMin, fMax));
                }

                return new LibSVMScalingFactor(lower, upper, featureRanges);
            }
        }
    }
}
