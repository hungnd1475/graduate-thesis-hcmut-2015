﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp.Helpers;

namespace HCMUT.EMRCorefResol.English.SVM
{
    using Utilities;

    public class EnglishSVMTrainer : ITrainer<SVMTrainingResult>
    {
        private static readonly SVMProblemScaler SCALER = new SVMProblemScaler();

        public SVMTrainingResult TrainFromDir(string emrDir, string conDir, string chainDir,
            IDataReader dataReader, IPreprocessor preprocessor)
        {
            throw new NotImplementedException();
        }

        public Task<SVMTrainingResult> TrainFromDirAsync(string emrDir, string conDir, string chainDir, 
            IDataReader dataReader, IPreprocessor preprocessor)
        {
            return Task.Run(() => TrainFromDir(emrDir, conDir, chainDir,
                dataReader, preprocessor));
        }

        public SVMTrainingResult TrainFromFile(string emrFile, string conFile, string chainFile,
            IDataReader dataReader, IPreprocessor preprocessor)
        {
            Timer.Start();

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
            var personPairSF = SCALER.Scale(problems.PersonPair, 0, 1);
            SVMProblemHelper.Save(problems.PersonPair, "Problems\\personpair.prb");
            //SVMProblemHelper.Save(problems.PronounInstance, "Problems\\pronouninstance.prb");
                        
            return new SVMTrainingResult(Timer.Stop(), new SVMClassifier(), problems);
        }

        public Task<SVMTrainingResult> TrainFromFileAsync(string emrFile, string conFile, string chainFile, 
            IDataReader dataReader, IPreprocessor preprocessor)
        {
            return Task.Run(() => TrainFromFile(emrFile, conFile, chainFile,
                dataReader, preprocessor));
        }
    }
}
