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
        public void Debug(string s)
        {
            SysDebug.WriteLine(s);
        }

        public void Error(string s)
        {
            Console.Error.WriteLine(s);
        }

        public void Info(string s)
        {
            Console.WriteLine(s);
        }
    }
}
