using HCMUT.EMRCorefResol.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class Logger
    {
        public static ILogger Current { get; set; } = new ConsoleLogger();
    }
}
