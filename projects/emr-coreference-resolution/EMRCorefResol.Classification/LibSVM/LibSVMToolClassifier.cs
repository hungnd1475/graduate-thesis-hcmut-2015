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
        private readonly string _modelsDir;

        public string ModelsDir { get { return _modelsDir; } }

        internal LibSVMToolClassifier(string modelsDir)
        {
            _modelsDir = modelsDir;
        }

        public LibSVMToolClassifier(XmlReader xmlReader, string dir)
        {
            _modelsDir = xmlReader.ReadElementContentAsString();
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

        public double[] Classify(Type instanceType, ClasProblem problem)
        {
            var name = instanceType.Name;
            var modelPath = Path.Combine(_modelsDir, $"{name}.model");
            var saveDir = Path.GetDirectoryName(modelPath);
            var tmpDir = Path.Combine(saveDir, "tmp");

            Directory.CreateDirectory(tmpDir);
            var rawPrbPath = Path.Combine(tmpDir, $"{name}-clas.prb");
            var scaledPrbPath = Path.Combine(tmpDir, $"{name}-clas.scaled");
            var outputPath = Path.Combine(tmpDir, $"{name}-clas.out");
            var sfPath = Path.Combine(_modelsDir, $"{name}.sf");

            // save
            ProblemSerializer.Serialize(problem, rawPrbPath);

            // scale
            LibSVMTools.RunSVMScale(sfPath, rawPrbPath, scaledPrbPath);

            // predict
            GetLogger().Info($"Classifying {name} problem...");
            LibSVMTools.RunSVMPredict(scaledPrbPath, modelPath, outputPath);

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

        public void WriteXml(XmlWriter writer, string dir)
        {
            writer.WriteElementString("ModelsDir", _modelsDir);
        }
    }
}
