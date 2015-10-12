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
        static EMRCollection EMR_COLLECTION = new EMRCollection(
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\docs",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\concepts",
            @"..\..\..\..\..\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train\chains");

        static void Main(string[] args)
        {
        }
    }
}
