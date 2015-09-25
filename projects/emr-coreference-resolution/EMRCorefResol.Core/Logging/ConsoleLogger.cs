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
        public void UpdateInfo(string s)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            WriteInfo(s, false);
        }

        public void WriteDebug(string s)
        {
            SysDebug.WriteLine(s);
        }

        public void WriteError(string s)
        {
            Console.Error.WriteLine(s);
        }

        public void WriteInfo(string s)
        {
            WriteInfo(s, true);
        }

        public void WriteInfo(string s, bool newLine)
        {
            if (newLine)
            {
                Console.WriteLine(s);
            }
            else
            {
                Console.Write(s);
            }
        }
    }
}
