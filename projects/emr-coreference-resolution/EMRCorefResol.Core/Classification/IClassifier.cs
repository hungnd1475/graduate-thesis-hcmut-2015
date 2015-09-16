using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HCMUT.EMRCorefResol
{
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
