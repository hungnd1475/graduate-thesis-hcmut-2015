using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp;

namespace HCMUT.EMRCorefResol.English.SVM
{
    class SVMProblemScaler
    {
        public SVMScalingFactor Scale(SVMProblem problem, double lower, double upper)
        {
            int size = problem.X[0].Length;
            var factor = new SVMScalingFactor.Range[size];

            for (int i = 0; i < size; i++)
            {
                factor[i] = new SVMScalingFactor.Range(double.MaxValue, double.MinValue);
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < problem.Length; j++)
                {
                    var v = problem.X[j][i].Value;
                    if (factor[i].Lower > v)
                        factor[i].Lower = v;
                    if (factor[i].Upper < v)
                        factor[i].Upper = v;
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < problem.Length; j++)
                {
                    if (factor[i].Upper > factor[i].Lower)
                    {
                        problem.X[j][i].Value = (problem.X[j][i].Value - factor[i].Lower) 
                            * (upper - lower) / (factor[i].Upper - factor[i].Lower) + lower;
                    }                    
                }
            }

            return new SVMScalingFactor(new SVMScalingFactor.Range(lower, upper), factor);
        }
    }
}
