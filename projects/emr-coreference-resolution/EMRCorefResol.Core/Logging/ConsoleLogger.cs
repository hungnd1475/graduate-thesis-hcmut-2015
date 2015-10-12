using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysDebug = System.Diagnostics.Debug;

namespace HCMUT.EMRCorefResol.Logging
{
    class ConsoleLogger : ILogger
    {
        public void Log(string s)
        {
            Console.WriteLine(s);
        }
    }
}
