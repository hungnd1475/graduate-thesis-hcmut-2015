using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        private readonly FeatureExtractingSystem _fExtractSystem = FeatureExtractingSystem.Instance;

        private TrainingSystem() { }

        public void TrainOne(string emrPath, string conceptsPath, string chainsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCollection = new ClasProblemCollection();
            _fExtractSystem.ExtractOne(emrPath, conceptsPath, chainsPath, dataReader, preprocessor,
                fExtractor, pCollection);
            Train(trainer, pCollection);           
        }

        public void TrainAll(string[] emrFiles, string[] conceptsFiles, string[] chainsFiles, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCollection = new ClasProblemCollection();
            _fExtractSystem.ExtractAll(emrFiles, conceptsFiles, chainsFiles, dataReader,
                preprocessor, fExtractor, pCollection);
            Train(trainer, pCollection);            
        }

        public void TrainCollection(EMRCollection emrCollection, IDataReader dataReader, IPreprocessor preprocessor,
            IFeatureExtractor fExtractor, ITrainer trainer)
        {
            var pCollection = new ClasProblemCollection();
            _fExtractSystem.ExtractCollection(emrCollection, dataReader, preprocessor,
                fExtractor, pCollection);
            Train(trainer, pCollection);            
        }

        private void Train(ITrainer trainer, ClasProblemCollection pCollection)
        {
            Console.WriteLine("Training...");
            trainer.Train<PersonPair>(pCollection.GetProblem<PersonPair>());
            trainer.Train<PersonInstance>(pCollection.GetProblem<PersonInstance>());
            trainer.Train<PronounInstance>(pCollection.GetProblem<PronounInstance>());
        }
    }
}
