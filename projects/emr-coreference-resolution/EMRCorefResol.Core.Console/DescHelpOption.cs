using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp.Internals;
using Fclp.Internals.Parsing;
using static System.Console;

namespace HCMUT.EMRCorefResol.Core.Console
{
    public static class DescHelpOption
    {
        public static void ShowHelp(IEnumerable<ICommandLineOption> options)
        {
            WriteLine("Usage:");
            foreach (var o in options)
            {               
                WriteLine($"-{o.ShortName}: {o.Description}");
            }
        }
    }
}
