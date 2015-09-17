using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HCMUT.EMRCorefResol
{
    /// <summary>
    /// Provides a common interface for classifiers.
    /// A concrete implementation must contains a constructor that takes a <see cref="XmlReader"/> parameter
    /// and a <see cref="string"/> parameter (in that order) for the <see cref="ClassifierSerializer"/> service to work properly.
    /// </summary>
    public interface IClassifier
    {
        double Classify(PersonPair instance, IFeatureVector f);
        double Classify(PersonInstance instance, IFeatureVector f);
        double Classify(ProblemPair instance, IFeatureVector f);
        double Classify(TreatmentPair instance, IFeatureVector f);
        double Classify(TestPair instance, IFeatureVector f);
        double Classify(PronounInstance instance, IFeatureVector f);

        void WriteXml(XmlWriter writer, string dir);
    }
}
