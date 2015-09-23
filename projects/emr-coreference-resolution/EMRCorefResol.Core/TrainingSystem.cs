using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            trainer.Train<PersonInstance>(pCreator.GetProblem<PersonInstance>());
            trainer.Train<PronounInstance>(pCreator.GetProblem<PronounInstance>());
        }

        public void TrainAll(string[] emrFiles, string[] conceptsFiles, string[] chainsFiles, IDataReader dataReader, 
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCreator = new ClasProblemCreator();

            for (int i = 0; i < emrFiles.Length; i++)
            {
                GetLogger().Info(Path.GetFileName(emrFiles[i]));

                var emr = new EMR(emrFiles[i], conceptsFiles[i], dataReader);
                var chains = new CorefChainCollection(chainsFiles[i], dataReader);

                fExtractor.EMR = emr;
                fExtractor.GroundTruth = chains;

                var instances = preprocessor.Process(emr);
                var features = new IFeatureVector[instances.Count];

                GetLogger().Info("Extracting features...");
                Parallel.For(0, instances.Count, k =>
                {
                    var t = instances[k];
                    features[k] = t.GetFeatures(fExtractor);
                });

                for (int k = 0; k < features.Length; k++)
                {
                    var fVector = features[k];
                    if (fVector != null)
                        instances[k].AddTo(pCreator, fVector);
                }
            }

            GetLogger().Info("Training...");

            trainer.Train<PersonPair>(pCreator.GetProblem<PersonPair>());
            trainer.Train<PersonInstance>(pCreator.GetProblem<PersonInstance>());
            trainer.Train<PronounInstance>(pCreator.GetProblem<PronounInstance>());

            //Directory.CreateDirectory("Problems");
            //trainer.ProblemSerializer.Serialize(pCreator.GetProblem<PersonPair>(), "Problems\\PersonPair.prb");
            //trainer.ProblemSerializer.Serialize(pCreator.GetProblem<PersonInstance>(), "Problems\\PersonInstance.prb");
            //trainer.ProblemSerializer.Serialize(pCreator.GetProblem<PronounInstance>(), "Problems\\PronounInstance.prb");
        }
    }
}
