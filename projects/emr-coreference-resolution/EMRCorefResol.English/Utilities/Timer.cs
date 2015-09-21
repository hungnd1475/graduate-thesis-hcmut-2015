using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HCMUT.EMRCorefResol.English.Utilities
{
    internal static class Timer
    {
        private static readonly Stopwatch StopWatch = new Stopwatch();

        public static void Start()
        {
            StopWatch.Start();
        }

        public static long Stop()
        {
            StopWatch.Stop();
            var elapse = StopWatch.ElapsedMilliseconds;
            StopWatch.Reset();
            return elapse;
        }
    }
}
