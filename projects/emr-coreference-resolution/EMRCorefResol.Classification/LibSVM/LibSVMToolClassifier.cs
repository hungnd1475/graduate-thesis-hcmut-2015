using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMToolClassifier : IClassifier
    {
        private readonly Dictionary<Type, string> _svmModels
            = new Dictionary<Type, string>();
        private readonly Dictionary<Type, string> _scalingFactors
            = new Dictionary<Type, string>();

        internal LibSVMToolClassifier(Dictionary<Type, string> svmModels,
            Dictionary<Type, string> scalingFactors)
        {
            _svmModels = svmModels;
            _scalingFactors = scalingFactors;
        }

        public IClasProblemSerializer ProblemSerializer
        {
            get { return LibSVMProblemSerializer.Instance; }
        }

        public double Classify(ProblemPair instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public double Classify(TestPair instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public double Classify(PronounInstance instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public double Classify(TreatmentPair instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public double Classify(PersonInstance instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public double Classify(PersonPair instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public double[] Classify<T>(ClasProblem problem) where T : IClasInstance
        {
            return Classify(typeof(T), problem);
        }

        public double[] Classify(Type instancetype, ClasProblem problem)
        {
            var modelPath = _svmModels[instancetype];
            var name = instancetype.Name;
            var saveDir = Path.GetDirectoryName(modelPath);
            var tmpDir = Path.Combine(saveDir, "tmp");

            Directory.CreateDirectory(tmpDir);
            var rawPrbPath = Path.Combine(tmpDir, $"{name}-clas.prb");
            var scaledPrbPath = Path.Combine(tmpDir, $"{name}-clas.scaled");
            var outputPath = Path.Combine(tmpDir, $"{name}-clas.out");
            var sfPath = _scalingFactors[instancetype];

            // save
            ProblemSerializer.Save(problem, rawPrbPath);

            // scale
            LibSVMTools.RunSVMScale(sfPath, rawPrbPath, scaledPrbPath);

            // predict
            GetLogger().Info($"Classifying {name} problem...");
            LibSVMTools.RunSVMPredict(scaledPrbPath, modelPath, outputPath);

            var target = new List<double>();
            var sr = new StreamReader(outputPath);
            while (!sr.EndOfStream)
            {
                var s = sr.ReadLine();
                double v;
                if (double.TryParse(s, out v))
                {
                    target.Add(v);
                }
            }
            sr.Close();

            //Directory.Delete(tmpDir, true); <- run this to delete tmp path, commented for now for debugging purpose.

            return target.ToArray();
        }

        public void WriteXml(XmlWriter writer, string dir)
        {
            throw new NotImplementedException();
        }
    }
}
