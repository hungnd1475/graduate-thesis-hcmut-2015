using HCMUT.EMRCorefResol.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Classification.LibSVM;
using HCMUT.EMRCorefResol.Utilities;
using System.Threading;

namespace HCMUT.EMRCorefResol.ConsoleTest
{
    using English;
    using System.IO;

    class Program
    {
        static string emrFile = @"..\..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
        static string conceptsFile = @"..\..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
        static string chainFile = @"..\..\..\..\..\dataset\Task_1C_Test_groundtruth\Tack_1C_to_be_released_10_02_2011\i2b2_Beth_Test\chains\clinical-3.txt.chains";

        static EMRCollection EMR_COLLECTION = new EMRCollection(
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\docs",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\concepts",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\chains");

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            //testPreprocessor();
            //testCorefChain();
            testFeatures();
            //testReadEMR();
            //var path = testTrainer();
            //testClassifier(path);
            //testTrainer();
            //testLoadClassifier();
            //testService();
            //var path = testTrainManyEMR(20);
            //testClassifier(@"Classification\LibSVMTools\Models\LibSVMTool.classifier", 10);
            //testClassifier(path, 1);
            testAhoCorasick();

            sw.Stop();
            Console.WriteLine($"Execution time: {sw.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }

        static EMR testReadEMR()
        {
            var emr = new EMR(emrFile, conceptsFile, new I2B2DataReader());

            for (int i = 0; i < emr.Concepts.Count; i++)
            {
                Console.WriteLine($"{emr.Concepts[i]}||t={emr.Concepts[i].Type.ToString().ToLower()}");
            }
            return emr;
        }

        static void testPreprocessor()
        {
            var emr = new EMR(emrFile, conceptsFile, new I2B2DataReader());
            var instances = new SimplePreprocessor().Process(emr);
            foreach (var i in instances)
            {
                Console.WriteLine(i);
            }
        }

        static void testCorefChain()
        {
            var chains = new CorefChainCollection(chainFile, new I2B2DataReader());
            foreach (var c in chains)
            {
                Console.WriteLine(c);
                Console.WriteLine("----------------------------------------");
            }
            Console.WriteLine(chains.GetPatientChain());
        }

        static void testFeatures()
        {
            var reader = new I2B2DataReader();
            var emr = new EMR(emrFile, conceptsFile, reader);
            var chains = new CorefChainCollection(chainFile, reader);
            var instances = new SimplePreprocessor().Process(emr);

            var extractor = new EnglishTrainingFeatureExtractor();
            extractor.EMR = emr;
            extractor.GroundTruth = chains;

            var features = new IFeatureVector[instances.Count];

            Parallel.For(0, instances.Count, k =>
            {
                var i = instances[k];
                features[k] = i.GetFeatures(extractor);
            });

            for (int i = 0; i < instances.Count; i++)
            {
                var f = features[i];
                if (f != null)
                    Print(instances[i], f);
            }
        }

        static string testTrainer()
        {
            var trainer = new LibSVMTrainer();
            var dataReader = new I2B2DataReader();
            var preprocessor = new SimplePreprocessor();
            var fExtractor = new EnglishTrainingFeatureExtractor();

            TrainingSystem.Instance.TrainOne(EMR_COLLECTION.GetEMRPath(0), EMR_COLLECTION.GetConceptsPath(0),
                EMR_COLLECTION.GetChainsPath(0), dataReader, preprocessor, fExtractor, trainer);

            var classifier = trainer.GetClassifier();
            var path = Path.Combine(trainer.ModelsDir, "LibSVMTool.classifier");
            ClassifierSerializer.Serialize(classifier, path);
            return path;
        }

        static string testTrainManyEMR(int size)
        {
            var trainer = new LibSVMTrainer();
            var dataReader = new I2B2DataReader();
            var preprocessor = new SimplePreprocessor();
            var fExtractor = new EnglishTrainingFeatureExtractor();

            string[] emr, concepts, chains;
            EMR_COLLECTION.GetRandom(size, out emr, out concepts, out chains);
            TrainingSystem.Instance.TrainAll(emr, concepts, chains, dataReader, preprocessor, fExtractor, trainer);

            var classifier = trainer.GetClassifier();
            var path = Path.Combine(trainer.ModelsDir, "LibSVMTool.classifier");
            ClassifierSerializer.Serialize(classifier, path);
            return path;
        }

        static void testClassifier(string classifierPath, int size)
        {
            var classifier = ClassifierSerializer.Deserialize(classifierPath);
            var dataReader = new I2B2DataReader();
            var preprocessor = new SimplePreprocessor();
            var fExtractor = new EnglishClasFeatureExtractor(classifier);

            var r = new Random();
            for (int i = 1; i <= size; i++)
            {
                var index = r.Next(1, EMR_COLLECTION.Count - 1);
                var emrPath = EMR_COLLECTION.GetEMRPath(index);
                Console.WriteLine($"{Path.GetFileName(emrPath)}");
                ClassificationSystem.Instance.ClassifyOne(emrPath, EMR_COLLECTION.GetConceptsPath(index),
                    EMR_COLLECTION.GetChainsPath(index), dataReader, preprocessor, fExtractor, classifier);
                classifier.ClearCache();
            }
        }

        static void testService()
        {
            Console.WriteLine("Annie gender: " + Service.English.getGender("annie"));
            Console.WriteLine(string.Join(" ", Service.English.getPOS("Annie goes to school")));
            Console.WriteLine(Service.English.getSyns("table")[1]);
        }

        static void testAhoCorasick()
        {
            var ac_kwd = new AhoCorasickKeywordDictionary(
                "it", "they", "them", "that",
                "which", "what", "who", "whom",
                "whose", "all", "any", "most",
                "some", "this", "that", "these",
                "those");

            var result = ac_kwd.Search("whose", KWSearchOptions.None);
            Console.WriteLine(string.Join(" ", result));
        }

        static void Print(IClasInstance i, IFeatureVector fVector)
        {
            Console.WriteLine($"{i}\nClass-Value:{fVector.ClassValue} {string.Join(" ", fVector.Select(f => $"{f.Name}:{f.Value}"))}\n");
        }
    }
}
