﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using System.IO;

namespace HCMUT.EMRCorefResol
{
    public class FeatureExtractingSystem
    {
        public static FeatureExtractingSystem Instance { get; } = new FeatureExtractingSystem();

        private FeatureExtractingSystem() { }

        public void ExtractOne(string emrPath, string conceptsPath, string chainsPath, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCollection pCreator,
            ISet<Type> extractTypes = null)
        {
            var gtExists = File.Exists(chainsPath);
            if (fExtractor.NeedGroundTruth && !gtExists)
                throw new ArgumentException("The feature extractor needs ground truth to operate properly.");

            Console.WriteLine(Path.GetFileName(emrPath));

            var emr = new EMR(emrPath, conceptsPath, dataReader);
            var chains = gtExists ? new CorefChainCollection(chainsPath, dataReader) : null;

            fExtractor.EMR = emr;
            fExtractor.GroundTruth = chains;

            var instances = preprocessor.Process(emr);
            var features = new IFeatureVector[instances.Count];
            int nDone = 0, iCount = instances.Count;

            Console.WriteLine("Extracting features...");

            Parallel.For(0, iCount, k =>
            {
                var t = instances[k];

                if (extractTypes == null || extractTypes.Count == 0 || extractTypes.Contains(t.GetType()))
                {
                    features[k] = t.GetFeatures(fExtractor);
                }

                lock (emr)
                {
                    nDone += 1;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"{nDone}/{iCount}");
                }
            });

            Console.WriteLine();

            for (int k = 0; k < features.Length; k++)
            {
                var fVector = features[k];
                if (fVector != null)
                    instances[k].AddTo(pCreator, fVector);
            }
        }

        public void ExtractAll(string[] emrPaths, string[] conceptsPaths, string[] chainsPaths, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCollection pCreator, 
            ISet<Type> extractTypes = null)
        {
            for (int i = 0; i < emrPaths.Length; i++)
            {
                ExtractOne(emrPaths[i], conceptsPaths[i], chainsPaths[i], dataReader,
                    preprocessor, fExtractor, pCreator, extractTypes);
            }
        }

        public void ExtractCollection(EMRCollection emrColl, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCollection pCreator,
            ISet<Type> extractTypes = null)
        {
            for (int i = 0; i < emrColl.Count; i++)
            {
                ExtractOne(emrColl.GetEMRPath(i), emrColl.GetConceptsPath(i), emrColl.GetChainsPath(i),
                    dataReader, preprocessor, fExtractor, pCreator, extractTypes);
            }
        }

        public void ExtractCollections(IEnumerable<EMRCollection> emrColls, IDataReader dataReader,
            IPreprocessor preprocessor, IFeatureExtractor fExtractor, ClasProblemCollection pCreator,
            ISet<Type> extractTypes = null)
        {
            foreach (var ec in emrColls)
            {
                ExtractCollection(ec, dataReader, preprocessor, fExtractor, pCreator, extractTypes);
            }
        }
    }
}
