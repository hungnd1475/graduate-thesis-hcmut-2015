using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp.Helpers;

namespace HCMUT.EMRCorefResol.English.SVM
{
    public class EnglishSVMTrainer : ITrainer
    {
        public TrainingResult TrainFromDir(string emrDir, string conDir, string chainDir,
            IDataReader dataReader, IPreprocessor preprocessor)
        {
            throw new NotImplementedException();
        }

        public TrainingResult TrainFromFile(string emrFile, string conFile, string chainFile,
            IDataReader dataReader, IPreprocessor preprocessor)
        {
            var emr = new EMR(emrFile, conFile, dataReader);
            var chains = new CorefChainCollection(chainFile, dataReader);
            var problems = new SVMProblems();

            var extractor = new EnglishTrainingFeatureExtractor();
            extractor.EMR = emr;
            extractor.GroundTruth = chains;

            var instances = preprocessor.Process(emr);
            var features = new ISVMTrainingFeatures[instances.Count];

            Parallel.For(0, instances.Count, k =>
            {
                var i = instances[k];
                features[k] = i.GetFeatures(extractor) as ISVMTrainingFeatures;
            });

            foreach (var f in features)
            {
                if (f != null)
                    f.AddTo(problems);
            }

            Directory.CreateDirectory("Problems");
            SVMProblemHelper.Save(problems.PersonPair, "Problems\\personpair.prb");
            SVMProblemHelper.Save(problems.PronounInstance, "Problems\\pronouninstance.prb");

            return null;
        }
    }
}
