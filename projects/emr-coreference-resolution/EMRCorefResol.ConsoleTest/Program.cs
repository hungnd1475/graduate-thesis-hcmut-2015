using HCMUT.EMRCorefResol.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HCMUT.EMRCorefResol.ConsoleTest
{
    using English;
    using English.SVM;

    class Program
    {
        static string emrFile = @"..\..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
        static string conceptsFile = @"..\..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
        static string chainFile = @"..\..\..\..\..\dataset\Task_1C_Test_groundtruth\Tack_1C_to_be_released_10_02_2011\i2b2_Beth_Test\chains\clinical-3.txt.chains";

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            //testPreprocessor();
            //testCorefChain();
            //testFeatures();
            //testReadEMR();
            //testTrainer();
            //testLoadClassifier();
            testService();

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

        static void testTrainer()
        {
            var trainer = new EnglishSVMTrainer();
            var result = trainer.TrainFromFile(emrFile, conceptsFile, chainFile, new I2B2DataReader(), new SimplePreprocessor());
            Console.WriteLine($"Completion Time: {result.CompletionTime}ms");
            ClassifierSerializer.Serialize(result.Classifier, "test.cls");
        }

        static void testLoadClassifier()
        {
            var c = ClassifierSerializer.Deserialize("test.cls");
        }

        static void testService()
        {
            Console.WriteLine("Annie gender: " + Service.English.getGender("annie"));
            Console.WriteLine(String.Join(" ", Service.English.getPOS("Annie goes to school")));
            Console.WriteLine(Service.English.getSyns("table")[1]);
        }

        static void Print(IClasInstance i, IFeatureVector fVector)
        {
            Console.WriteLine($"{i}\nClass-Value:{fVector.ClassValue} {string.Join(" ", fVector.Select(f => $"{f.Name}:{f.Value}"))}\n");
        }
    }
}
