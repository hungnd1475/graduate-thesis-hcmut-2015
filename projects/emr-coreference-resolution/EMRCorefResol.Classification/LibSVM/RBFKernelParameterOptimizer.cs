using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class RBFKernelParameterOptimizer
    {
        private const int PROBLEM_MAX_LENGTH = 20000;

        public bool Optimize(GridSearchConfig config, string problemPath, int nfold,
            out double cost, out double gamma, out double accuracy)
        {
            var content = File.ReadAllText(problemPath);
            var problem = LibSVM.ReadProblem(content, null);
            content = null;

            return Optimize(config, problem, nfold, out cost, out gamma, out accuracy);
        }

        internal bool Optimize(GridSearchConfig config, SVMProblem problem, int nfold,
            out double cost, out double gamma, out double accuracy)
        {
            LibSVM.SetPrintStringFunction(new LibSVMPrintFunction(DummyPrint));
            AccuracyInfo bestAcc = null;
            string info;

            if (problem.Length <= PROBLEM_MAX_LENGTH)
            {
                info = "Entering first phase of grid search operation...";
                Console.WriteLine(info);
                Logger.Current.Log(info);

                var accInfos = Search(config, problem, nfold);
                bestAcc = FindBest(accInfos);
            }

            if (bestAcc != null)
            {
                info = $"Best region: Log2C={bestAcc.Cost}, Log2G={bestAcc.Gamma}, Accuracy: {bestAcc.Accuracy}\n";
                Console.WriteLine(info);
                Logger.Current.Log(info);

                if (config.SearchBestRegion)
                {
                    info = "Entering second phase of grid search operation...";
                    Console.WriteLine(info);
                    Logger.Current.Log(info);

                    var r = Math.Abs(config.BestRegionRadius);
                    var s = Math.Abs(config.BestRegionStep);

                    var bestRegionConfig = new GridSearchConfig(
                        Range.Create(bestAcc.Cost - r, bestAcc.Cost + r), s,
                        Range.Create(bestAcc.Gamma - r, bestAcc.Gamma + r), s,
                        false);

                    var accInfos = Search(bestRegionConfig, problem, nfold);
                    bestAcc = FindBest(accInfos);

                    if (bestAcc != null)
                    {
                        cost = bestAcc.Cost;
                        gamma = bestAcc.Gamma;
                        accuracy = bestAcc.Accuracy;
                        return true;
                    }
                }
                else
                {
                    cost = bestAcc.Cost;
                    gamma = bestAcc.Gamma;
                    accuracy = bestAcc.Accuracy;
                    return true;
                }
            }

            cost = -1d;
            gamma = -1d;
            accuracy = -1d;
            return false;
        }

        private SVMProblem CreateSubProblem(SVMProblem problem, int length)
        {
            if (problem.Length <= length)
                return problem;

            return Enumerable.Range(0, length).Aggregate(new SVMProblem(),
                (SVMProblem p, int i) =>
                {
                    p.Add(problem.X[i], problem.Y[i]);
                    return p;
                });
        }

        private AccuracyInfo[] Search(GridSearchConfig config, SVMProblem problem, int nfold)
        {
            var accInfos = CreateGrid(config);
            int[] labels; double[] weights;
            LibSVM.CalcWeights(problem, out labels, out weights);

            Parallel.For(0, accInfos.Length, (i) =>
            {
                var a = accInfos[i];
                var param = CreateParameter(Math.Pow(2, a.Cost), Math.Pow(2, a.Gamma), labels, weights);
                var target = LibSVM.CrossValidation(param, problem, nfold);
                a.Accuracy = Enumerable.Range(0, target.Length).Aggregate(0, (count, k) =>
                {
                    return target[k] == problem.Y[k] ? count + 1 : count;
                });
                a.Accuracy /= problem.Length;
                Logger.Current.Log($"Log2C = {a.Cost}  \tLog2G = {a.Gamma}  \tAccuracy = {a.Accuracy * 100}%");
            });

            return accInfos;
        }

        private SVMParameter CreateParameter(double cost, double gamma, int[] labels, double[] weights)
        {
            return new SVMParameter()
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernel.RBF,
                C = cost,
                Gamma = gamma,
                Shrinking = false
            };
        }

        private AccuracyInfo[] CreateGrid(GridSearchConfig config)
        {
            var cCount = (int)((config.CostRange.Max - config.CostRange.Min) / config.CostStep + 1);
            var gCount = (int)((config.GammaRange.Max - config.GammaRange.Min) / config.GammaStep + 1);
            var accInfo = new AccuracyInfo[cCount * gCount];
            int i = 0;

            for (var c = config.CostRange.Min; c <= config.CostRange.Max; c += config.CostStep)
            {
                for (var g = config.GammaRange.Min; g <= config.GammaRange.Max; g += config.GammaStep)
                {
                    accInfo[i++] = new AccuracyInfo()
                    {
                        Cost = c,
                        Gamma = g,
                        Accuracy = 0
                    };
                }
            }

            return accInfo;
        }

        private AccuracyInfo FindBest(AccuracyInfo[] accInfos)
        {
            AccuracyInfo bestAcc = null;
            for (int i = 0; i < accInfos.Length; i++)
            {
                var a = accInfos[i];
                if (bestAcc == null || bestAcc.Accuracy < a.Accuracy)
                {
                    bestAcc = a;
                }
            }
            return bestAcc;
        }

        private void DummyPrint(string s)
        {
            // do nothing
        }

        class AccuracyInfo
        {
            public double Cost { get; set; }
            public double Gamma { get; set; }
            public double Accuracy { get; set; }
        }
    }
}
