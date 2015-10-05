using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;

namespace HCMUT.EMRCorefResol
{
    using Classification;
    using System.IO;

    public class TrainingSystem
    {
        private static readonly List<Type> INSTANCE_TYPES =
            new List<Type>()
            {
                typeof(PersonPair),
                typeof(PersonInstance),
                typeof(PronounInstance),
                typeof(ProblemPair),
                typeof(TestPair),
                typeof(TreatmentPair)
            };

        public static TrainingSystem Instance { get; } = new TrainingSystem();

        private TrainingSystem() { }

        public void TrainOne(string emrPath, string conceptsPath, string chainsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCreator = new ClasProblemCreator();
            ExtractFeatures(emrPath, conceptsPath, chainsPath, dataReader,
                    preprocessor, fExtractor, pCreator);
            GetLogger().WriteInfo("Training...");

            trainer.Train<PersonPair>(pCreator.GetProblem<PersonPair>());
            trainer.Train<PersonInstance>(pCreator.GetProblem<PersonInstance>());
            trainer.Train<PronounInstance>(pCreator.GetProblem<PronounInstance>());
        }

        public void TrainAll(string[] emrFiles, string[] conceptsFiles, string[] chainsFiles, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCreator = new ClasProblemCreator();

            for (int i = 0; i < emrFiles.Length; i++)
            {
                ExtractFeatures(emrFiles[i], conceptsFiles[i], chainsFiles[i], dataReader,
                    preprocessor, fExtractor, pCreator);
            }

            GetLogger().WriteInfo("Training...");

            trainer.Train<PersonPair>(pCreator.GetProblem<PersonPair>());
            trainer.Train<PersonInstance>(pCreator.GetProblem<PersonInstance>());
            trainer.Train<PronounInstance>(pCreator.GetProblem<PronounInstance>());
        }

        public void TrainAll(EMRCollection emrCollection, IDataReader dataReader, IPreprocessor preprocessor,
            IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCreator = new ClasProblemCreator();

            for (int i = 0; i < emrCollection.Count; i++)
            {
                ExtractFeatures(emrCollection.GetEMRPath(i), emrCollection.GetConceptsPath(i), emrCollection.GetChainsPath(i),
                    dataReader, preprocessor, fExtractor, pCreator);
            }

            GetLogger().WriteInfo("Training...");

            trainer.Train<PersonPair>(pCreator.GetProblem<PersonPair>());
            trainer.Train<PersonInstance>(pCreator.GetProblem<PersonInstance>());
            trainer.Train<PronounInstance>(pCreator.GetProblem<PronounInstance>());
        }

        private void ExtractFeatures(string emrPath, string conceptsPath, string chainsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCreator pCreator)
        {
            GetLogger().WriteInfo(Path.GetFileName(emrPath));

            var emr = new EMR(emrPath, conceptsPath, dataReader);
            var chains = new CorefChainCollection(chainsPath, dataReader);

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

                var t = instances[k];
                features[k] = t.GetFeatures(fExtractor);
            });

            GetLogger().WriteInfo("\n");

            for (int k = 0; k < features.Length; k++)
            {
                var fVector = features[k];
                if (fVector != null)
                    instances[k].AddTo(pCreator, fVector);
            }
        }
    }
}
