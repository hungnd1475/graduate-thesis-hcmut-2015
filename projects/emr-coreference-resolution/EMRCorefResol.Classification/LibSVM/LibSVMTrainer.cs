using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;
using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMTrainer : ITrainer
    {
        private readonly LibSVMClassifier _classifier;

        private readonly string _saveDir;
        private readonly string _problemDir, _modelDir;
        private readonly GridSearchConfig _gridSearchConfig = null;

        public string ModelsDir { get { return _modelDir; } }

        public LibSVMTrainer(string saveDir, GridSearchConfig gridSearchConfig)
        {
            _gridSearchConfig = gridSearchConfig;
            _saveDir = saveDir;
            _problemDir = Path.Combine(_saveDir, "Problems");
            _modelDir = Path.Combine(_saveDir, "Models");

            Directory.CreateDirectory(_problemDir);
            Directory.CreateDirectory(_modelDir);

            _classifier = new LibSVMClassifier(_modelDir);
        }

        public LibSVMTrainer(GridSearchConfig gridSearchConfig)
            : this("Classification\\LibSVMTools", gridSearchConfig)
        { }

        public LibSVMTrainer(string saveDir)
            : this(saveDir, null)
        { }

        public LibSVMTrainer() : this("Classification\\LibSVMTools") { }

        public IClasProblemSerializer ProblemSerializer
        {
            get { return LibSVMProblemSerializer.Instance; }
        }

        public IClassifier GetClassifier()
        {
            return _classifier;
        }

        public void Train(Type instanceType, ClasProblem problem)
        {
            var name = instanceType.Name;

            var rawPrbPath = Path.Combine(_problemDir, $"{name}-training.prb");
            var scaledPrbPath = Path.Combine(_problemDir, $"{name}-training.scaled");
            var sfPath = Path.Combine(_modelDir, $"{name}.sf");
            var modelPath = Path.Combine(_modelDir, $"{name}.model");

            ProblemSerializer.Serialize(problem, rawPrbPath);

            // scale
            var scaledData = LibSVM.RunSVMScale(0d, 1d, sfPath, rawPrbPath);

            // optimize parameter for rbf kernel
            GetLogger().WriteInfo($"Performing grid search on {name} problem...");

            double cost = 1, gamma = 1d / problem.X[0].Length;
            double accuracy = -1d;
            if (_gridSearchConfig != null)
            {
                accuracy = RBFParameterOptimizer.Optimize(_gridSearchConfig, 5, scaledData, problem.Y.ToArray(), out cost, out gamma);
            }

            using (var sr = new StreamWriter(scaledPrbPath))
            {
                sr.Write(scaledData);
                scaledData = string.Empty;
            }

            // train
            if (accuracy != -1d)
            {
                GetLogger().WriteInfo($"Grid search best result: a={accuracy * 100}% at c={cost} g={gamma}");
                GetLogger().WriteInfo($"Training {name} problem using the above parameters...");
                LibSVM.RunSVMTrainRBFKernel(Math.Pow(2, cost), Math.Pow(2, gamma), scaledPrbPath, modelPath, true);
            }
            else
            {
                GetLogger().WriteInfo("Grid search failed!");
                GetLogger().WriteInfo($"Training {name} problem using default parameters...");
                LibSVM.RunSVMTrainRBFKernel(scaledPrbPath, modelPath, true);
            }
        }

        public void Train<T>(ClasProblem problem) where T : IClasInstance
        {
            Train(typeof(T), problem);
        }

        private static class RBFParameterOptimizer
        {
            private const int PROBLEM_MAX_LENGTH = 80000;

            public static double Optimize(GridSearchConfig config, int nfold, string data, double[] y, out double cost, out double gamma)
            {
                LibSVM.SetPrintStringFunction(new LibSVMPrintFunction(DummyPrint));
                var allProblem = LibSVM.ReadProblem(data, null);
                AccuracyInfo bestAcc = null;

                if (allProblem.Length > PROBLEM_MAX_LENGTH)
                {
                    var subProblem = LibSVM.ReadProblem(data, PROBLEM_MAX_LENGTH);
                    var accInfos = Search(config, nfold, subProblem);
                    bestAcc = FindBest(accInfos);
                }
                else
                {
                    var accInfos = Search(config, nfold, allProblem);
                    bestAcc = FindBest(accInfos);
                }

                if (bestAcc != null)
                {
                    var bestRegionConfig = new GridSearchConfig(
                        Range.Create(bestAcc.Cost - 2, bestAcc.Cost + 2), 0.25d,
                        Range.Create(bestAcc.Gamma - 2, bestAcc.Gamma + 2), 0.25d);

                    var accInfos = Search(bestRegionConfig, nfold, allProblem);
                    bestAcc = FindBest(accInfos);

                    if (bestAcc != null)
                    {
                        cost = bestAcc.Cost;
                        gamma = bestAcc.Gamma;
                        return (double)bestAcc.CorrectCount / y.Length;
                    }
                }

                cost = -1d;
                gamma = -1d;
                return -1d;
            }

            private static AccuracyInfo[] Search(GridSearchConfig config, int nfold, SVMProblem problem)
            {
                var accInfos = CreateGrid(config);
                Parallel.For(0, accInfos.Length, (i) =>
                {
                    var a = accInfos[i];
                    var target = LibSVM.RBFKernelCrossValidation(Math.Pow(2, a.Cost), Math.Pow(2, a.Gamma), nfold, problem);
                    a.CorrectCount = Enumerable.Range(0, target.Length).Where(k => target[k] == problem.Y[k]).Count();
                });
                return accInfos;
            }

            private static AccuracyInfo[] CreateGrid(GridSearchConfig config)
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
                            CorrectCount = 0
                        };
                    }
                }

                return accInfo;
            }

            private static AccuracyInfo FindBest(AccuracyInfo[] accInfos)
            {
                AccuracyInfo bestAcc = null;
                for (int i = 0; i < accInfos.Length; i++)
                {
                    var a = accInfos[i];
                    if (bestAcc == null || bestAcc.CorrectCount < a.CorrectCount)
                    {
                        bestAcc = a;
                    }
                }
                return bestAcc;
            }

            private static void DummyPrint(string s)
            {
                // do nothing
            }

            class AccuracyInfo
            {
                public double Cost { get; set; }
                public double Gamma { get; set; }
                public int CorrectCount { get; set; }
            }
        }
    }
}
