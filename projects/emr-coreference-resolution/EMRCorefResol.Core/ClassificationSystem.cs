using HCMUT.EMRCorefResol.Classification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class ClassificationSystem
    {
        public static ClassificationSystem Instance { get; } = new ClassificationSystem();

        private readonly FeatureExtractingSystem _fExtractingSystem =
            FeatureExtractingSystem.Instance;

        private ClassificationSystem() { }

        public void ClassifyOne(string emrPath, string conceptsPath, string chainsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, IClassifier classifier)
        {
            var pCreator = new ClasProblemCollection();
            _fExtractingSystem.ExtractOne(emrPath, conceptsPath, chainsPath, dataReader,
                preprocessor, fExtractor, pCreator);

            Console.WriteLine("Classifying...");
            classifier.Classify<PersonInstance>(pCreator.GetProblem<PersonInstance>());
            classifier.Classify<PersonPair>(pCreator.GetProblem<PersonPair>());
            classifier.Classify<PronounInstance>(pCreator.GetProblem<PronounInstance>());
        }
    }
}
