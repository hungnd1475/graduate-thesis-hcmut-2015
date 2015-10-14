using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Classification.LibSVM;

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
                Console.WriteLine(parseResult.ErrorText);
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

            var trainer = APISelector.SelectTrainer(args.ClasMethod, args.OutputDir);
            var instanceType = APISelector.SelectInstanceType(args.Instance);
            trainer.Train(instanceType, args.FeaturePath, config);
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.FeaturePath)
                .As('d', "data")
                .Required();

            p.Setup(a => a.ClasMethod)
                .As('m', "clasmethod")
                .SetDefault(ClasMethod.LibSVM);

            p.Setup(a => a.Instance)
                .As('i', "instance")
                .Required();

            p.Setup(a => a.Gamma)
                .As('g', "gamma")
                .SetDefault(0);

            p.Setup(a => a.Cost)
                .As('c', "cost")
                .SetDefault(0);

            p.Setup(a => a.OutputDir)
                .As('o', "outdir")
                .Required();

            p.Setup(a => a.ShouldLog)
                .As('l', "log")
                .SetDefault(-1);

            p.Setup(a => a.ApplyWeights)
                .As('w', "weights")
                .SetDefault(-1);

            return p;
        }
    }
}
