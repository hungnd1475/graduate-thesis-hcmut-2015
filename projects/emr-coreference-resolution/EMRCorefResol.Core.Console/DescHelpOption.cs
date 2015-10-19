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
    public class DescHelpOption : IHelpCommandLineOption
    {
        public bool ShouldShowHelp(IEnumerable<ParsedOption> parsedOptions, StringComparison comparisonType)
        {
            return true;
        }

        public void ShowHelp(IEnumerable<ICommandLineOption> options)
        {
            foreach (var o in options)
            {               
                WriteLine($"-{o.ShortName}: {o.Description}");
            }
        }
    }
}
