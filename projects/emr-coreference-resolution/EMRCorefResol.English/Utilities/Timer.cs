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
        private static readonly Stopwatch STOP_WATCH = new Stopwatch();

        public static void Start()
        {
            STOP_WATCH.Start();
        }

        public static long Stop()
        {
            STOP_WATCH.Stop();
            var elapse = STOP_WATCH.ElapsedMilliseconds;
            STOP_WATCH.Reset();
            return elapse;
        }
    }
}
