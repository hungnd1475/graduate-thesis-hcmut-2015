using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;

namespace HCMUT.EMRCorefResol.RBFOptimizingConsole
{
    using Classification.LibSVM;
    using Logging;
    using System.IO;
    using Core.Console;

    class Program
    {
        static void Main(string[] rawArgs)
        {
            var parser = PrepareParser();
            var result = parser.Parse(rawArgs);

            if (result.HasErrors)
            {
                DescHelpOption.ShowHelp(parser.Options);
                return;
            }
            else if (result.HelpCalled)
            {
                return;
            }

            var args = parser.Object;
            Logger.Current = new FileLogger(args.LogFile);

            var rawCost = args.CostRange.Split(',').Select(c => double.Parse(c.Trim())).ToArray();
            var rawGamma = args.GammaRange.Split(',').Select(c => double.Parse(c.Trim())).ToArray();
            var bestRegion = args.BestRegion.Split(',').Select(c => double.Parse(c.Trim())).ToArray();

            var costRange = Range.Create(rawCost[0], rawCost[1]);
            var gammaRange = Range.Create(rawGamma[0], rawGamma[1]);

            var config = new GridSearchConfig(costRange, rawCost[2], gammaRange, rawGamma[2], 
                Convert.ToBoolean(args.SearchBestRegion), bestRegion[0], bestRegion[1]);

            double cost, gamma, accuracy;
            Console.WriteLine("Optimizing...");
            new RBFKernelParameterOptimizer().Optimize(config, args.DataPath, args.NFold, out cost, out gamma, out accuracy);

            Logger.Current.Log($"Result: Log2C={cost}, Log2G={gamma}, Accuracy={accuracy * 100:N3}%");
            Console.WriteLine("Done! See log file for details.");
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.DataPath)
                .As('d', "data")
                .Required()
                .WithDescription("Set data file path (required).");

            p.Setup(a => a.NFold)
                .As('n', "nfold")
                .SetDefault(5)
                .WithDescription("Set number of fold (default 5).");

            p.Setup(a => a.CostRange)
                .As('c', "cost")
                .SetDefault("-5,15,2")
                .WithDescription("Set C range and step, syntax [min],[max],[step] (default -5,15,2).");

            p.Setup(a => a.GammaRange)
                .As('g', "gamma")
                .SetDefault("-15,3,2")
                .WithDescription("Set gamma range and step, syntax [min],[max],[step] (default -15,3,2).");

            p.Setup(a => a.SearchBestRegion)
                .As('b', "searchbest")
                .SetDefault(1).
                WithDescription("Whether the optimizer should perform best region search (default 1).");

            p.Setup(a => a.BestRegion)
                .As('r', "region")
                .SetDefault("2,0.25")
                .WithDescription("Set best region radius and step, syntax [radius],[step] (default 2,0.5).");

            p.Setup(a => a.LogFile)
                .As('l', "log")
                .SetDefault("log.txt")
                .WithDescription("Set log file path (default log.txt)");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }
    }
}
