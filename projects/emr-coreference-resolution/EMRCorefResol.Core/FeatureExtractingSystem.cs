using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;
using System.IO;

namespace HCMUT.EMRCorefResol
{
    public class FeatureExtractingSystem
    {
        public static FeatureExtractingSystem Instance { get; } = new FeatureExtractingSystem();

        private FeatureExtractingSystem() { }

        public void ExtractOne(string emrPath, string conceptsPath, string chainsPath, string medicationsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCreator pCreator)
        {
            var gtExists = File.Exists(chainsPath);
            if (fExtractor.NeedGroundTruth && !gtExists)
                throw new ArgumentException("The feature extractor needs ground truth to operate properly.");

            GetLogger().WriteInfo(Path.GetFileName(emrPath));

            var emr = new EMR(emrPath, conceptsPath, medicationsPath, dataReader);
            var chains = gtExists ? new CorefChainCollection(chainsPath, dataReader) : null;

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

        public void ExtractAll(string[] emrPaths, string[] conceptsPaths, string[] chainsPaths, string[] medicationsPaths, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCreator pCreator)
        {
            for (int i = 0; i < emrPaths.Length; i++)
            {
                ExtractOne(emrPaths[i], conceptsPaths[i], chainsPaths[i], medicationsPaths[i], dataReader,
                    preprocessor, fExtractor, pCreator);
            }
        }

        public void ExtractCollection(EMRCollection emrColl, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCreator pCreator)
        {
            for (int i = 0; i < emrColl.Count; i++)
            {
                ExtractOne(emrColl.GetEMRPath(i), emrColl.GetConceptsPath(i), emrColl.GetChainsPath(i), emrColl.GetMedicationsPath(i),
                    dataReader, preprocessor, fExtractor, pCreator);
            }
        }

        public void ExtractCollections(IEnumerable<EMRCollection> emrColls, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCreator pCreator)
        {
            foreach (var ec in emrColls)
            {
                ExtractCollection(ec, dataReader, preprocessor, fExtractor, pCreator);
            }
        }
    }
}
