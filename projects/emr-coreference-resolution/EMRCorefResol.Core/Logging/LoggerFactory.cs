using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Logging
{
    public static class LoggerFactory
    {
        private static ILogger CURRENT_LOGGER = new ConsoleLogger();

        public static ILogger GetLogger()
        {
            return CURRENT_LOGGER;
        }

        public static void RegisterLogger(ILogger logger)
        {
            CURRENT_LOGGER = logger;
        }
    }
}
