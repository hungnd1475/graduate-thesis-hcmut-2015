using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMClassifier : IClassifier
    {
        private static readonly Type[] InstanceTypes = new[]
        {
            typeof(PersonInstance),
            typeof(PersonPair),
            typeof(PronounInstance),
            typeof(ProblemPair),
            typeof(TestPair),
            typeof(TreatmentPair)
        };

        private readonly string _modelsDir;
        private readonly ICache<Concept, ClasResult> _cache
            = new UnlimitedCache<Concept, ClasResult>();

        private readonly object _syncRoot = new object();

        private readonly Dictionary<Type, SVMModel> _svmModels
            = new Dictionary<Type, SVMModel>();

        private readonly Dictionary<Type, LibSVMScalingFactor> _svmScalingFactors
            = new Dictionary<Type, LibSVMScalingFactor>();

        public string ModelsDir { get { return _modelsDir; } }

        public LibSVMClassifier(string modelsDir)
        {
            _modelsDir = modelsDir;

            Console.WriteLine("Loading models...");
            foreach (var t in InstanceTypes)
            {
                var modelPath = Path.Combine(_modelsDir, $"{t.Name}.model");
                var model = LibSVM.LoadModel(modelPath);
                _svmModels.Add(t, model);

                var sfPath = Path.Combine(_modelsDir, $"{t.Name}.sf");
                var sf = LibSVMScalingFactor.Load(sfPath);
                _svmScalingFactors.Add(t, sf);
            }
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
            return _cache.GetValue(instance.Concept, c =>
            {
                return ClassifyInstance(instance, f);
            });
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
            //var sfPath = Path.Combine(_modelsDir, $"{instanceType.Name}.sf");
            //var tmpDir = Path.Combine(_modelsDir, "tmp");

            //var pCreator = new ClasProblemCollection();
            //pCreator.Add(instance, fVector);
            //var rawPrb = pCreator.GetProblem(instanceType);

            //string scaledPrbContent;
            //lock (_syncRoot)
            //{
            //    Directory.CreateDirectory(tmpDir);
            //    var tmpPrbPath = Path.Combine(_modelsDir, "tmp", $"{instanceType.Name}.prb");
            //    ProblemSerializer.Serialize(rawPrb, tmpPrbPath);
            //    scaledPrbContent = LibSVM.RunSVMScale(sfPath, tmpPrbPath);
            //}

            //var scaledPrb = LibSVM.ReadProblem(scaledPrbContent);
            //var nodes = scaledPrb.X[0];

            var sf = _svmScalingFactors[instanceType];            
            var svmModel = _svmModels[instanceType];

            if (sf != null && svmModel != null)
            {
                var nodes = Scale(fVector, sf);
                double confidence;
                var label = LibSVM.Predict(svmModel, nodes, out confidence);
                return new ClasResult(label, confidence);
            }
            else
            {
                return null;
            }
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
                        nodes.Add(new SVMNode(index + 1, v));
                    }
                }
            }

            return nodes.ToArray();
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
