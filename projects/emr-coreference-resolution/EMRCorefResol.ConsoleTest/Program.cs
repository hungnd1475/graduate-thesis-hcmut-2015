using HCMUT.EMRCorefResol.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //testPreprocessor();
            testCorefChain();
            Console.ReadLine();
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
    }
}
