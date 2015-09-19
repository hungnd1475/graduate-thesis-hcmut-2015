using LibSVMsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.SVM
{
    static class Helpers
    {
        public static SVMNode[] ToSVMNodes(this IFeatureVector f)
        {
            var nodes = new SVMNode[f.Size];
            for (int i = 0; i < f.Size; i++)
            {
                if (f[i] != null)
                    nodes[i] = new SVMNode(i + 1, f[i].Value);
            }
            return nodes;
        }

        public static SVMScalingFactor ScaleTo(this SVMProblem problem, double lower, double upper)
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

        public static void ScaleBy(this SVMProblem problem, SVMScalingFactor factor)
        {
            if (problem.Length < 0)
                return;

            if (factor.FeatureRanges.Length != problem.X[0].Length)
                throw new InvalidOperationException("Scaling factors is not matched with problems.");

            for (int i = 0; i < problem.Length; i++)
            {
                problem.X[i].ScaleBy(factor);
            }
        }

        public static void ScaleBy(this SVMNode[] nodes, SVMScalingFactor factor)
        {
            if (factor.FeatureRanges.Length != nodes.Length)
                throw new InvalidOperationException("Scaling factors is not matched with features.");

            for (int i = 0; i < nodes.Length; i++)
            {
                if (factor.FeatureRanges[i].Upper > factor.FeatureRanges[i].Lower)
                {
                    nodes[i].Value = (nodes[i].Value - factor.FeatureRanges[i].Lower)
                        * (factor.NewRange.Upper - factor.NewRange.Lower) / (factor.FeatureRanges[i].Upper - factor.FeatureRanges[i].Lower)
                        + factor.NewRange.Lower;
                }
            }
        }
    }
}
