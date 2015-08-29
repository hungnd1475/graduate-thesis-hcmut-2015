using HCMUT.EMRCorefResol.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.ConsoleTest
{
    using Training.English;

    class Program
    {
        static void Main(string[] args)
        {
            //testPreprocessor();
            //testCorefChain();
            testFeatures();
            //testReadEMR();
            Console.ReadLine();
        }

        static EMR testReadEMR()
        {
            string emrFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
            string conceptsFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
            var emr = new EMR(emrFile, conceptsFile, new I2B2DataReader());
            Console.WriteLine(emr.Content);
            Console.WriteLine(emr.Concepts[1]);

            var bIndex = emr.BeginIndexOf(emr.Concepts[1]);
            var eIndex = emr.EndIndexOf(emr.Concepts[1]);
            Console.WriteLine(bIndex);
            Console.WriteLine(eIndex);
            Console.WriteLine(emr.Content.Substring(bIndex, eIndex - bIndex + 1));
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

            foreach (var i in instances)
            {
                var f = i.GetFeatures(extractor);
                if (f != null)
                    Print(i, f);
            }           
        }

        static void Print(IClasInstance i, IFeatureVector fVector)
        {
            Console.WriteLine($"{i}---{string.Join(" ", fVector.Select(f => $"{f.Name}:{f.Value}"))}");
        }
    }
}
