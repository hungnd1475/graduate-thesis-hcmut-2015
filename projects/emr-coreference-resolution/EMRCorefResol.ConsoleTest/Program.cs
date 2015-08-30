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

    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            //testPreprocessor();
            //testCorefChain();
            testFeatures();
            //testReadEMR();

            sw.Stop();
            Console.WriteLine($"Execution time: {sw.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }

        static EMR testReadEMR()
        {
            string emrFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
            string conceptsFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
            var emr = new EMR(emrFile, conceptsFile, new I2B2DataReader());
            
            for (int i = 0; i < emr.Concepts.Count; i++)
            {
                Console.WriteLine($"{emr.Concepts[i]}||t={emr.Concepts[i].Type.ToString().ToLower()}");
            }
            return emr;
        }

        static void testPreprocessor()
        {
            string emrFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
            string conceptsFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
            var emr = new EMR(emrFile, conceptsFile, new I2B2DataReader());
            var instances = new SimplePreprocessor().Process(emr);
            foreach (var i in instances)
            {
                Console.WriteLine(i);
            }
        }

        static void testCorefChain()
        {
            string chainFile = @"..\..\..\..\dataset\Task_1C_Test_groundtruth\Tack_1C_to_be_released_10_02_2011\i2b2_Beth_Test\chains\clinical-3.txt.chains";
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
            string emrFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
            string conceptsFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
            string chainFile = @"..\..\..\..\dataset\Task_1C_Test_groundtruth\Tack_1C_to_be_released_10_02_2011\i2b2_Beth_Test\chains\clinical-3.txt.chains";

            var reader = new I2B2DataReader();
            var emr = new EMR(emrFile, conceptsFile, reader);
            var chains = new CorefChainCollection(chainFile, reader);
            var instances = new SimplePreprocessor().Process(emr);
            var extractor = new EnglishTrainingFeatureExtractor(emr, chains);

            Parallel.ForEach(instances, i =>
            {
                var f = i.GetFeatures(extractor);
                if (f != null)
                {
                    Print(i, f);
                }
            });
        }

        static void Print(IClasInstance i, IFeatureVector fVector)
        {
            Console.WriteLine($"{i}\nClass-Value:{fVector.ClassValue} {string.Join(" ", fVector.Select(f => $"{f.Name}:{f.Value}"))}\n");
        }
    }
}
