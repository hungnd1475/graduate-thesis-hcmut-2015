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
            testPreprocessor();
            Console.ReadLine();
        }

        static void testPreprocessor()
        {
            string emrFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
            string conceptsFile = @"..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
            var emr = new EMR(emrFile, conceptsFile);
            var instances = new SimplePreprocessor().Process(emr);
            foreach (var i in instances)
            {
                Console.WriteLine(i);
            }
        }
    }
}
