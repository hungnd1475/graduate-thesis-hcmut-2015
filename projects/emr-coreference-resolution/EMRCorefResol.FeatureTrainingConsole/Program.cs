using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using Fclp.Internals;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Classification.LibSVM;
using Fclp.Internals.Parsing;

namespace HCMUT.EMRCorefResol.FeatureTrainingConsole
{
    class Program
    {
        static void Main(string[] rawArgs)
        {
            var argsParser = PrepareParser();
            var parseResult = argsParser.Parse(rawArgs);

            if (parseResult.HasErrors)
            {
                DescHelpOption.ShowHelp(argsParser.Options);
                return;
            }
            else if (parseResult.HelpCalled)
            {
                return;
            }

            var args = argsParser.Object;

            if (!File.Exists(args.FeaturePath))
            {
                Console.WriteLine("Features file not found.");
                return;
            }

            GenericConfig config = null;

            if (args.ShouldLog >= 0)
            {
                config = new GenericConfig();
                config.SetConfig(LibSVMConfig.ShouldLog, Convert.ToBoolean(args.ShouldLog));
            }

            if (args.Cost > 0)
            {
                config = config ?? new GenericConfig();
                config.SetConfig(LibSVMConfig.Cost, args.Cost);
            }

            if (args.Gamma > 0)
            {
                config = config ?? new GenericConfig();
                config.SetConfig(LibSVMConfig.Gamma, args.Gamma);
            }

            if (args.ApplyWeights >= 0)
            {
                config = config ?? new GenericConfig();
                config.SetConfig(LibSVMConfig.ApplyWeights, Convert.ToBoolean(args.ApplyWeights));
            }

            if (args.CacheSize > 0)
            {
                config = config ?? new GenericConfig();
                config.SetConfig(LibSVMConfig.CacheSize, args.CacheSize);
            }

            var trainer = APISelector.SelectTrainer(args.ClasMethod, args.OutputDir);
            var instanceType = APISelector.SelectInstanceType(args.Instance);
            trainer.Train(instanceType, args.FeaturePath, config);
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();
                        
            p.Setup(a => a.FeaturePath)
                .As('d', "data")
                .Required()
                .WithDescription("Set data file path (required).");

            p.Setup(a => a.ClasMethod)
                .As('m', "clasmethod")
                .SetDefault(ClasMethod.LibSVM)
                .WithDescription(Descriptions.ClasMethod("1"));

            p.Setup(a => a.Instance)
                .As('i', "instance")
                .Required()
                .WithDescription(Descriptions.Instance(null));

            p.Setup(a => a.Gamma)
                .As('g', "gamma")
                .SetDefault(0)
                .WithDescription("Set parameter gamma for kernel function (LibSVM only, default 1/num_features).");

            p.Setup(a => a.Cost)
                .As('c', "cost")
                .SetDefault(0)
                .WithDescription("Set parameter C (LibSVM only, default 1).");

            p.Setup(a => a.OutputDir)
                .As('o', "outdir")
                .Required()
                .WithDescription("Set output directory path (required).");

            p.Setup(a => a.ShouldLog)
                .As('l', "log")
                .SetDefault(-1)
                .WithDescription("Whether the trainer should print log (default 0).");

            p.Setup(a => a.ApplyWeights)
                .As('w', "weights")
                .SetDefault(-1)
                .WithDescription("Whether the trainer should apply class weights (LibSVM only, default 0).");

            p.Setup(a => a.CacheSize)
                .As('s', "cache")
                .SetDefault(-1)
                .WithDescription("Set cache size in MB (LibSVM only, default 100)");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }
    }
}
