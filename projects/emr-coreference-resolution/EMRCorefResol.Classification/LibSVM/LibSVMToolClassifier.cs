using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMToolClassifier : IClassifier
    {
        internal readonly Dictionary<Type, string> SVMModels
            = new Dictionary<Type, string>();
        internal readonly Dictionary<Type, string> ScalingFactors
            = new Dictionary<Type, string>();

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
            throw new NotImplementedException();
        }

        public double[] Classify(Type instancetype, ClasProblem problem)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer, string dir)
        {
            throw new NotImplementedException();
        }
    }
}
