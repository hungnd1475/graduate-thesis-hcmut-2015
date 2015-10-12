using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HCMUT.EMRCorefResol.Classification
{
    /// <summary>
    /// Provides a common interface for classifiers.
    /// A concrete implementation must be a public class and must contain a constructor that takes a <see cref="XmlReader"/> parameter
    /// and a <see cref="string"/> parameter (in that order) for the <see cref="ClassifierSerializer"/> service to work properly.
    /// </summary>
    public interface IClassifier
    {
        ClasResult Classify(PersonPair instance, IFeatureVector f);
        ClasResult Classify(PersonInstance instance, IFeatureVector f);
        ClasResult Classify(ProblemPair instance, IFeatureVector f);
        ClasResult Classify(TreatmentPair instance, IFeatureVector f);
        ClasResult Classify(TestPair instance, IFeatureVector f);
        ClasResult Classify(PronounInstance instance, IFeatureVector f);

        double[] Classify<T>(ClasProblem problem) where T : IClasInstance;
        double[] Classify(Type instanceType, ClasProblem problem);

        IClasProblemSerializer ProblemSerializer { get; }

        string ModelsDir { get; }

        void ClearCache();
    }
}
