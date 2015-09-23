using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;

namespace HCMUT.EMRCorefResol
{
    using Classification;

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
            IPreprocessor preprocessor, ITrainingFeatureExtractor fExtractor, ITrainer trainer) 
        {
            var emr = new EMR(emrPath, conceptsPath, dataReader);
            var chains = new CorefChainCollection(chainsPath, dataReader);
            var pCreator = new ClasProblemCreator();

            fExtractor.EMR = emr;
            fExtractor.GroundTruth = chains;

            var instances = preprocessor.Process(emr);
            var features = new IFeatureVector[instances.Count];

            GetLogger().Info("Extracting features...");
            Parallel.For(0, instances.Count, k =>
            {
                var i = instances[k];
                features[k] = i.GetFeatures(fExtractor);
            });

            for (int i = 0; i < features.Length; i++)
            {
                var fVector = features[i];
                if (fVector != null)
                    instances[i].AddTo(pCreator, fVector);
            }

            GetLogger().Info("Training...");
            trainer.Train<PersonPair>(pCreator.GetProblem<PersonPair>());
        }
    }
}
