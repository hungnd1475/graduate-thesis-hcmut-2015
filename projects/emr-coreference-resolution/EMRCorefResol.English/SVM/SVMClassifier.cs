using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using LibSVMsharp;

namespace HCMUT.EMRCorefResol.English.SVM
{
    public class SVMClassifier : IClassifier
    {
        private string test;
        private readonly SVMModel _personPairModel, _personInstanceModel,
            _pronounInstanceModel, _problemPairModel, _testPairModel, _treatmentPairModel;

        public SVMClassifier(XmlReader reader, string dir)
        {
            test = reader.ReadElementContentAsString();
        }

        internal SVMClassifier(SVMModel personPairModel, SVMModel personInstanceModel,
            SVMModel pronounInstanceModel, SVMModel problemPairModel,
            SVMModel testPairModel, SVMModel treatmentPairModel)
        {
            _personPairModel = personPairModel;
            _personInstanceModel = personInstanceModel;
            _pronounInstanceModel = pronounInstanceModel;
            _problemPairModel = problemPairModel;
            _testPairModel = testPairModel;
            _treatmentPairModel = treatmentPairModel;
        }

        internal SVMClassifier() { }

        public double Classify(PersonInstance instance, IFeatureVector f)
        {
            throw new NotImplementedException();
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

        public double Classify(PersonPair instance, IFeatureVector f)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(XmlWriter writer, string dir)
        {
            writer.WriteStartElement("PersonPair");
            writer.WriteString("path to personpair svm model");
            writer.WriteEndElement();
        }
    }
}
