using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;
using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMClassifier : IClassifier, IDisposable
    {
        private readonly string _modelsDir;
        private readonly Dictionary<Concept, double> _cache
            = new Dictionary<Concept, double>();

        private readonly object _syncRoot = new object();

        private readonly Dictionary<Type, SVMModel> _svmModels
            = new Dictionary<Type, SVMModel>();

        public string ModelsDir { get { return _modelsDir; } }

        internal LibSVMClassifier(string modelsDir)
        {
            _modelsDir = modelsDir;
        }

        public LibSVMClassifier(XmlReader xmlReader, string dir)
        {
            _modelsDir = xmlReader.ReadElementContentAsString();
        }

        public IClasProblemSerializer ProblemSerializer
        {
            get { return LibSVMProblemSerializer.Instance; }
        }

        public double Classify(ProblemPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public double Classify(TestPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public double Classify(PronounInstance instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public double Classify(TreatmentPair instance, IFeatureVector f)
        {
            return ClassifyInstance(instance, f);
        }

        public double Classify(PersonInstance instance, IFeatureVector f)
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

        public double Classify(PersonPair instance, IFeatureVector f)
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
            GetLogger().WriteInfo($"Classifying {name} problem...");
            LibSVM.RunSVMPredict(scaledPrbPath, modelPath, outputPath, true);

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

        private double ClassifyInstance(IClasInstance instance, IFeatureVector fVector)
        {
            var instanceType = instance.GetType();
            var sfPath = Path.Combine(_modelsDir, $"{instanceType.Name}.sf");

            var pCreator = new ClasProblemCreator();
            pCreator.Add(instance, fVector);
            var rawPrb = pCreator.GetProblem(instanceType);

            string scaledPrbContent;
            lock (_syncRoot)
            {
                var tmpPrbPath = Path.Combine(_modelsDir, $"{instanceType.Name}-tmp.prb");
                ProblemSerializer.Serialize(rawPrb, tmpPrbPath);
                scaledPrbContent = LibSVM.RunSVMScale(sfPath, tmpPrbPath);
            }

            var scaledPrb = LibSVM.ReadProblem(scaledPrbContent, null);
            var svmModel = GetModel(instanceType);
            return LibSVM.Predict(svmModel, scaledPrb.X[0]);
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

        public void WriteXml(XmlWriter writer, string dir)
        {
            writer.WriteElementString("ModelsDir", _modelsDir);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
