using HCMUT.EMRCorefResol.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;

namespace HCMUT.EMRCorefResol
{
    public class ClassificationSystem
    {
        public static ClassificationSystem Instance { get; } = new ClassificationSystem();

        private ClassificationSystem() { }

        public void ClassifyOne(string emrPath, string conceptsPath, string chainsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, IClassifier classifier)
        {
            var emr = new EMR(emrPath, conceptsPath, dataReader);
            var chains = new CorefChainCollection(chainsPath, dataReader);
            var pCreator = new ClasProblemCreator();

            fExtractor.EMR = emr;
            fExtractor.GroundTruth = chains;

            var instances = preprocessor.Process(emr);
            var features = new IFeatureVector[instances.Count];
            int nDone = 0, iCount = instances.Count;

            GetLogger().WriteInfo("Extracting features...");
            Parallel.For(0, iCount, k =>
            {
                lock (emr)
                {
                    nDone += 1;
                    GetLogger().UpdateInfo($"{nDone}/{iCount}");
                }

                var i = instances[k];
                features[k] = i.GetFeatures(fExtractor);
            });

            GetLogger().WriteInfo("\n");

            for (int i = 0; i < features.Length; i++)
            {
                var fVector = features[i];
                if (fVector != null)
                    instances[i].AddTo(pCreator, fVector);
            }

            classifier.Classify<PersonInstance>(pCreator.GetProblem<PersonInstance>());

            classifier.Classify<PersonPair>(pCreator.GetProblem<PersonPair>());

            classifier.Classify<PronounInstance>(pCreator.GetProblem<PronounInstance>());
        }
    }
}
