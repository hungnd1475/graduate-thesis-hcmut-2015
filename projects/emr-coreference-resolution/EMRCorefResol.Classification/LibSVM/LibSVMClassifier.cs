using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMClassifier : IClassifier
    {
        private readonly string _modelsDir;
        private readonly Dictionary<Concept, ClasResult> _cache
            = new Dictionary<Concept, ClasResult>();

        private readonly object _syncRoot = new object();

        private readonly Dictionary<Type, SVMModel> _svmModels
            = new Dictionary<Type, SVMModel>();

        private readonly Dictionary<Type, LibSVMScalingFactor> _svmScalingFactors
            = new Dictionary<Type, LibSVMScalingFactor>();

        public string ModelsDir { get { return _modelsDir; } }

        public LibSVMClassifier(string modelsDir)
        {
            _modelsDir = modelsDir;
        }

        public IClasProblemSerializer ProblemSerializer
        {
            get { return LibSVMProblemSerializer.Instance; }
        }

        public ClasResult Classify(ProblemPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public ClasResult Classify(TestPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public ClasResult Classify(PronounInstance instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public ClasResult Classify(TreatmentPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public ClasResult Classify(PersonInstance instance, IFeatureVector f)
        {
            lock (_syncRoot)
            {
                if (!_cache.ContainsKey(instance.Concept))
                {
                    _cache.Add(instance.Concept, ClassifyInstance(instance, f));
                }
            }
            return _cache[instance.Concept];
        }

        public ClasResult Classify(PersonPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f); ;
        }

        public double[] Classify<T>(ClasProblem problem) where T : IClasInstance
        {
            return Classify(typeof(T), problem);
        }

        public double[] Classify(Type instanceType, ClasProblem problem)
        {
            var name = instanceType.Name;
            var modelPath = Path.Combine(_modelsDir, $"{name}.model");
            var tmpDir = Path.Combine(_modelsDir, "tmp");

            Directory.CreateDirectory(tmpDir);
            var rawPrbPath = Path.Combine(tmpDir, $"{name}-clas.prb");
            var scaledPrbPath = Path.Combine(tmpDir, $"{name}-clas.scaled");
            var outputPath = Path.Combine(tmpDir, $"{name}-clas.out");
            var sfPath = Path.Combine(_modelsDir, $"{name}.sf");

            // save
            ProblemSerializer.Serialize(problem, rawPrbPath);

            // scale
            LibSVM.RunSVMScale(sfPath, rawPrbPath, scaledPrbPath);

            // predict
            Console.WriteLine($"Classifying {name} problem...");
            LibSVM.RunSVMPredict(scaledPrbPath, modelPath, outputPath);

            var target = new double[problem.Size];
            var sr = new StreamReader(outputPath);

            for (int i = 0; !sr.EndOfStream && i < problem.Size; i++)
            {
                var s = sr.ReadLine();
                double v;
                if (double.TryParse(s, out v))
                {
                    target[i] = v;
                }
            }
            sr.Close();

            // TODO: run the line below to delete tmp path, commented for now for debugging purpose
            //Directory.Delete(tmpDir, true);

            return target;
        }

        private ClasResult ClassifyInstance(IClasInstance instance, IFeatureVector fVector)
        {
            var instanceType = instance.GetType();
            var sfPath = Path.Combine(_modelsDir, $"{instanceType.Name}.sf");
            var tmpDir = Path.Combine(_modelsDir, "tmp");

            var pCreator = new ClasProblemCollection();
            pCreator.Add(instance, fVector);
            var rawPrb = pCreator.GetProblem(instanceType);

            string scaledPrbContent;
            lock (_syncRoot)
            {
                Directory.CreateDirectory(tmpDir);
                var tmpPrbPath = Path.Combine(_modelsDir, "tmp", $"{instanceType.Name}.prb");
                ProblemSerializer.Serialize(rawPrb, tmpPrbPath);
                scaledPrbContent = LibSVM.RunSVMScale(sfPath, tmpPrbPath);
            }
            var scaledPrb = LibSVM.ReadProblem(scaledPrbContent);

            //var sf = GetScalingFactor(instanceType);
            //var nodes = Scale(fVector, sf);
            var nodes = scaledPrb.X[0];
            var svmModel = GetModel(instanceType);

            double confidence;
            var label = LibSVM.Predict(svmModel, nodes, out confidence);
            return new ClasResult(label, confidence);
        }

        private static SVMNode[] Scale(IFeatureVector fVector, LibSVMScalingFactor scalingFactors)
        {
            var nodes = new List<SVMNode>();
            var index = -1;

            for (int i = 0; i < fVector.Size; i++)
            {
                var f = fVector[i];
                for (int j = 0; j < f.Value.Length; j++)
                {
                    index += 1;
                    var v = scalingFactors.Scale(index, f.Value[j]);

                    if (v != 0)
                    {
                        nodes.Add(new SVMNode(index, v));
                    }
                }
            }

            return nodes.ToArray();
        }

        private SVMModel GetModel(Type instanceType)
        {
            lock (_syncRoot)
            {
                if (!_svmModels.ContainsKey(instanceType))
                {
                    var modelPath = Path.Combine(_modelsDir, $"{instanceType.Name}.model");
                    var model = LibSVM.LoadModel(modelPath);
                    _svmModels.Add(instanceType, model);
                }
            }
            return _svmModels[instanceType];
        }

        private LibSVMScalingFactor GetScalingFactor(Type instanceType)
        {
            lock (_syncRoot)
            {
                if (!_svmScalingFactors.ContainsKey(instanceType))
                {
                    var sfPath = Path.Combine(_modelsDir, $"{instanceType.Name}.sf");
                    var sf = new LibSVMScalingFactor(sfPath);
                    _svmScalingFactors.Add(instanceType, sf);
                }
            }
            return _svmScalingFactors[instanceType];
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
