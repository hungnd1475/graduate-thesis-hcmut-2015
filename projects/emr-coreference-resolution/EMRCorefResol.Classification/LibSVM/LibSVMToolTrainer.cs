using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMToolTrainer : ITrainer<LibSVMToolClassifier>
    {
        private readonly LibSVMToolClassifier _classifier = new LibSVMToolClassifier();
        private readonly string _saveDir = "SVM\\LibSVMTools";
        private readonly string _problemDir, _modelDir;

        public LibSVMToolTrainer(string saveDir)
        {
            _saveDir = saveDir;
            _problemDir = Path.Combine(_saveDir, "Problems");
            _modelDir = Path.Combine(_saveDir, "Models");
        }

        public LibSVMToolTrainer()
        {
            _problemDir = Path.Combine(_saveDir, "Problems");
            _modelDir = Path.Combine(_saveDir, "Models");
        }

        public IClasProblemSerializer ProblemSerializer
        {
            get { return LibSVMProblemSerializer.Instance; }
        }

        public LibSVMToolClassifier GetClassifier()
        {
            return _classifier;
        }

        public void Train(Type instanceType, ClasProblem problem)
        {
            var name = instanceType.Name;
            Directory.CreateDirectory(_problemDir);
            Directory.CreateDirectory(_modelDir);

            var rawPrbPath = Path.Combine(_problemDir, $"{name}.prb");
            var scaledPrbPath = Path.Combine(_problemDir, $"{name}.scaled");
            var sfPath = Path.Combine(_modelDir, $"{name}.sf");
            var modelPath = Path.Combine(_modelDir, $"{name}.model");

            ProblemSerializer.Save(problem, rawPrbPath);

            // scale
            Console.WriteLine($"Scaling {name} problem...");
            var pInfo = new ProcessStartInfo()
            {
                FileName = LibSVMTools.SVMScalePath,
                Arguments = $"-l 0 -u 1 -s {sfPath} {rawPrbPath}",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var p = Process.Start(pInfo);

            var sw = new StreamWriter(scaledPrbPath);
            sw.Write(p.StandardOutput.ReadToEnd());
            sw.Close();

            // train
            Console.WriteLine($"Training {name} problem...");
            pInfo = new ProcessStartInfo()
            {
                FileName = LibSVMTools.SVMTrainPath,
                Arguments = $"-s 0 -t 2 {scaledPrbPath} {modelPath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            p = Process.Start(pInfo);
            p.WaitForExit();

            _classifier.SVMModels.Add(instanceType, modelPath);
            _classifier.ScalingFactors.Add(instanceType, sfPath);
        }

        public void Train<T>(ClasProblem problem) where T : IClasInstance
        {
            Train(typeof(T), problem);
        }
    }
}
