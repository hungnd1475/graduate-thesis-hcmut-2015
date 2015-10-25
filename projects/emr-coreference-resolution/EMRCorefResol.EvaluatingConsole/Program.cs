using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Evaluations;

namespace HCMUT.EMRCorefResol.EvaluatingConsole
{
    class Program
    {
        static readonly IPerfMetric[] PerfMetrics =
        {
            new MUCPerfMetric(),
            new BCubedPerfMetric(),
            new CEAFPerfMetric()
        };

        static readonly ConceptType[] Types =
        {
            ConceptType.None,
            ConceptType.Person,
            ConceptType.Problem,
            ConceptType.Test,
            ConceptType.Treatment
        };

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

            if (!Directory.Exists(args.GroundTruthDir))
            {
                Console.WriteLine("Ground truth directory not found.");
                return;
            }

            if (!Directory.Exists(args.SystemChainsDir))
            {
                Console.WriteLine("System chains directory not found.");
                return;
            }

            if (!string.IsNullOrEmpty(args.ChainsFile))
            {
                var gtFile = Path.Combine(args.GroundTruthDir, args.ChainsFile);
                var scFile = Path.Combine(args.SystemChainsDir, args.ChainsFile);

                if (!File.Exists(gtFile))
                {
                    Console.WriteLine($"Ground truth file {args.ChainsFile} not found.");
                    return;
                }

                if (!File.Exists(scFile))
                {

                }
            }
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.GroundTruthDir)
                .As('g', "groundtruth")
                .Required()
                .WithDescription("Set ground truth directory (required).");

            p.Setup(a => a.SystemChainsDir)
                .As('s', "systemchains")
                .Required()
                .WithDescription("Set system chains directory (required).");

            p.Setup(a => a.Format)
                .As('f', "format")
                .SetDefault(EMRFormat.I2B2)
                .WithDescription(Descriptions.EMRFormat("1"));

            p.Setup(a => a.OutputFile)
                .As('o', "output")
                .Required()
                .WithDescription("Set output file path (required).");

            p.Setup(a => a.ChainsFile)
                .As('c', "chains")
                .SetDefault(null)
                .WithDescription("Set chains file name to evaluate (optional).");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }
    }
}
