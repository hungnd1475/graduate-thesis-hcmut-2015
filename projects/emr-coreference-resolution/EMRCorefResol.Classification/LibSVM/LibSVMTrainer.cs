using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMTrainer : ITrainer
    {
        private readonly LibSVMClassifier _classifier;

        private readonly string _saveDir;
        private readonly string _problemDir, _modelDir;
        private readonly GridSearchConfig _gridSearchConfig = null;
        private readonly RBFKernelParameterOptimizer _rbfOptimizer
            = new RBFKernelParameterOptimizer();

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
            ProblemSerializer.Serialize(problem, rawPrbPath);
            Train(instanceType, rawPrbPath);
        }

        public void Train<T>(ClasProblem problem) where T : IClasInstance
        {
            Train(typeof(T), problem);
        }

        public void Train<T>(string problemPath) where T : IClasInstance
        {
            Train(typeof(T), problemPath);
        }

        public void Train(Type instanceType, string problemPath)
        {
            var name = instanceType.Name;
            Console.WriteLine($"Preparing training {name} problem...");

            var scaledPrbPath = Path.Combine(_problemDir, $"{name}-training.scaled");
            var sfPath = Path.Combine(_modelDir, $"{name}.sf");
            var modelPath = Path.Combine(_modelDir, $"{name}.model");

            // scale
            Console.WriteLine("Scaling problem...");
            var scaledData = LibSVM.RunSVMScale(0d, 1d, sfPath, problemPath);
            var problem = LibSVM.ReadProblem(scaledData, null);

            Console.WriteLine("Calculating cost weights...");
            int[] labels; double[] weights;
            CalcWeights(problem, out labels, out weights);
            Console.WriteLine("Cost weights: " +
                $"{string.Join(" ", Enumerable.Range(0, labels.Length).Select(i => $"{labels[i]}:{weights[i]}"))}");

            var svmParam = new SVMParameter()
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernel.RBF,
                C = 1,
                Gamma = 1d / problem.X[0].Length,
                Probability = true,
                Shrinking = false
            };

            if (_gridSearchConfig != null)
            {
                // optimize parameter for rbf kernel
                Console.WriteLine($"Performing grid search ({problem.Length} instances)...");

                double cost = 1, gamma = 1d / problem.X[0].Length;
                double accuracy = -1d;

                if (_rbfOptimizer.Optimize(_gridSearchConfig, problem, 3, out cost, out gamma, out accuracy))
                {
                    Console.WriteLine($"Grid search result: a={accuracy * 100}% at c={cost} g={gamma}");
                    Console.WriteLine("Training using the above parameters...");

                    svmParam.C = Math.Pow(2, cost);
                    svmParam.Gamma = Math.Pow(2, gamma);
                }
                else
                {
                    Console.WriteLine("Grid search failed!");
                    Console.WriteLine("Training using default parameters...");
                }
            }
            else
            {
                Console.WriteLine("Training using default parameters...");
            }

            problem = null;
            using (var sr = new StreamWriter(scaledPrbPath))
            {
                sr.Write(scaledData);
                scaledData = string.Empty;
            }

            // train
            LibSVM.RunSVMTrain(svmParam, scaledPrbPath, modelPath, false);
            Console.WriteLine("Done!");
        }

        private static void CalcWeights(SVMProblem problem, out int[] labels, out double[] weights)
        {
            var classCounts = new Dictionary<int, int>();
            foreach (var y in problem.Y)
            {
                var yInt = (int)y;
                if (!classCounts.ContainsKey(yInt))
                {
                    classCounts.Add(yInt, 0);
                }
                else
                {
                    classCounts[yInt] += 1;
                }
            }

            var maxCount = classCounts.Values.Aggregate(int.MinValue, (max, count) => max < count ? count : max);
            labels = classCounts.Keys.ToArray();
            weights = classCounts.Values.Select(count => (double)maxCount / count).ToArray();
        }
    }
}
