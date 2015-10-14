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

    class Program
    {
        static void Main(string[] rawArgs)
        {
            Console.WriteLine("Optimizing...");

            var parser = PrepareParser();
            var result = parser.Parse(rawArgs);

            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                return;
            }

            var args = parser.Object;
            Logger.Current = new FileLogger(args.LogFile);

            var rawCost = args.CostRange.Split(',').Select(c => double.Parse(c)).ToArray();
            var rawGamma = args.GammaRange.Split(',').Select(c => double.Parse(c)).ToArray();
            var bestRegion = args.BestRegion.Split(',').Select(c => double.Parse(c)).ToArray();

            var costRange = Range.Create(rawCost[0], rawCost[1]);
            var gammaRange = Range.Create(rawGamma[0], rawGamma[1]);

            var config = new GridSearchConfig(costRange, rawCost[2], gammaRange, rawGamma[2], args.SearchBestRegion,
                bestRegion[0], bestRegion[1]);

            double cost, gamma, accuracy;
            new RBFKernelParameterOptimizer().Optimize(config, args.DataPath, args.NFold, out cost, out gamma, out accuracy);

            Logger.Current.Log($"Result: Log2C={cost}, Log2G={gamma}, Accuracy={accuracy * 100:N3}%");
            Console.WriteLine("Done! See log file for details.");
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.DataPath)
                .As('d', "data")
                .Required();

            p.Setup(a => a.NFold)
                .As('n', "nfold")
                .SetDefault(5);

            p.Setup(a => a.CostRange)
                .As('c', "cost")
                .SetDefault("-5,15,2");

            p.Setup(a => a.GammaRange)
                .As('g', "gamma")
                .SetDefault("-15,3,2");

            p.Setup(a => a.SearchBestRegion)
                .As('b', "searchbest")
                .SetDefault(true);

            p.Setup(a => a.BestRegion)
                .As('r', "region")
                .SetDefault("2,0.25");

            p.Setup(a => a.LogFile)
                .As('l', "log")
                .SetDefault("log.txt");

            return p;
        }
    }
}
