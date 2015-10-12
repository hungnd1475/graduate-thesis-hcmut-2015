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

            var trainer = SelectTrainer(args);
            var instanceType = SelectInstanceType(args);
            trainer.Train(instanceType, args.FeaturePath);

            Console.ReadLine();
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.FeaturePath)
                .As('d', "featpath")
                .SetDefault(null);

            p.Setup(a => a.ClasMethod)
                .As('c', "clasmethod")
                .SetDefault(ClasMethod.LibSVM);

            p.Setup(a => a.Instance)
                .As('i', "instance")
                .Required();

            p.Setup(a => a.GammaRange)
                .As('g', "gamma")
                .SetDefault(null);

            p.Setup(a => a.CostRange)
                .As('s', "cost")
                .SetDefault(null);

            p.Setup(a => a.OutputDir)
                .As('o', "output")
                .Required();

            p.Setup(a => a.SearchBestRegion)
                .As('b', "bestregion")
                .SetDefault(false);

            return p;
        }

        static ITrainer SelectTrainer(Arguments args)
        {
            ITrainer trainer = null;

            if (args.ClasMethod == ClasMethod.LibSVM)
            {
                GridSearchConfig config = null;
                if (args.CostRange != null && args.GammaRange != null)
                {
                    var costRange = args.CostRange.Split(',');
                    var gammaRange = args.GammaRange.Split(',');
                    config = new GridSearchConfig(
                        Range.Create(double.Parse(costRange[0]), double.Parse(costRange[1])), double.Parse(costRange[2]),
                        Range.Create(double.Parse(gammaRange[0]), double.Parse(gammaRange[1])), double.Parse(gammaRange[2]),
                        args.SearchBestRegion);
                }
                trainer = new LibSVMTrainer(args.OutputDir, config);
            }

            return trainer;
        }

        static Type SelectInstanceType(Arguments args)
        {
            switch (args.Instance)
            {
                case Instance.PersonInstance:
                    return typeof(PersonInstance);
                case Instance.PersonPair:
                    return typeof(PersonPair);
                case Instance.ProblemPair:
                    return typeof(ProblemPair);
                case Instance.PronounInstance:
                    return typeof(PronounInstance);
                case Instance.TestPair:
                    return typeof(TestPair);
                case Instance.TreatmentPair:
                    return typeof(TreatmentPair);
                default:
                    throw new ArgumentException("args");
            }
        }
    }
}
