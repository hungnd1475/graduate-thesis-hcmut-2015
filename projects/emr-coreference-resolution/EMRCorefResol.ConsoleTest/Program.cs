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
using Fclp;

namespace HCMUT.EMRCorefResol.ConsoleTest
{
    using English;
    using System.IO;

    class Program
    {
        static string emrFile = @"..\..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\docs\clinical-3.txt";
        static string conceptsFile = @"..\..\..\..\..\dataset\Task_1C\i2b2_Test\i2b2_Beth_Test\concepts\clinical-3.txt.con";
        static string chainFile = @"..\..\..\..\..\dataset\Task_1C_Test_groundtruth\Tack_1C_to_be_released_10_02_2011\i2b2_Beth_Test\chains\clinical-3.txt.chains";
        static string medicationFile = @"..\..\..\..\..\dataset\Task_1C_Test_groundtruth\Tack_1C_to_be_released_10_02_2011\i2b2_Beth_Test\medications\clinical-3.txt";

        static EMRCollection EMR_COLLECTION = new EMRCollection(
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\docs",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\concepts",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\chains",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\medications");

        static void Main(string[] args)
        {            
        }
    }
}
