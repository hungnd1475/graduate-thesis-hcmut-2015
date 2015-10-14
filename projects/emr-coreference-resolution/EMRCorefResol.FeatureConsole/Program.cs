using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Classification;
using System.IO;
using HCMUT.EMRCorefResol.Core.Console;

namespace HCMUT.EMRCorefResol.FeatureConsole
{
    class Program
    {
        static void Main(string[] rawArgs)
        {
            var argsParser = PrepareParser();
            var parseResult = argsParser.Parse(rawArgs);

            if (parseResult.HasErrors)
            {
                Console.Write(parseResult.ErrorText);
                Console.ReadLine();
                return;
            }

            var args = argsParser.Object;
            var emrCollections = args.EMRDirs.Split('|').Select(ep => new EMRCollection(ep));

            if (args.Mode != Mode.Classify && emrCollections.Any(e => !e.HasGroundTruth))
            {
                Console.Write("EMR paths must contain chains directories in Train or Test mode");
                Console.ReadLine();
                return;
            }

            if (args.Mode != Mode.Train && !Directory.Exists(args.ModelsDir))
            {
                Console.Write("Model file must be specified in Test or Classify mode.");
                Console.ReadLine();
                return;
            }

            var pCreator = new ClasProblemCollection();
            var dataReader = APISelector.SelectDataReader(args.EMRFormat);
            var fExtractor = APISelector.SelectFeatureExtractor(args.Language, args.Mode, 
                args.ClasMethod, args.ModelsDir);
            var preprocessor = new SimplePreprocessor();

            if (args.Random > 0)
            {
                // TODO: refine this
                string[] emrPaths, conceptsPaths, chainsPaths;
                emrCollections.First().GetRandom(args.Random, out emrPaths, out conceptsPaths, out chainsPaths);
                FeatureExtractingSystem.Instance.ExtractAll(emrPaths, conceptsPaths, chainsPaths, dataReader,
                    preprocessor, fExtractor, pCreator);
            }
            else
            {
                FeatureExtractingSystem.Instance.ExtractCollections(emrCollections, dataReader, preprocessor,
                    fExtractor, pCreator);
            }

            var serializer = APISelector.SelectProblemSerializer(args.ClasMethod);
            Directory.CreateDirectory(args.OutDir);

            Console.WriteLine("Serializing...");
            serializer.Serialize(pCreator.GetProblem<PersonPair>(), Path.Combine(args.OutDir, "PersonPair.prb"), args.DoScale);
            serializer.Serialize(pCreator.GetProblem<PersonInstance>(), Path.Combine(args.OutDir, "PersonInstance.prb"), args.DoScale);
            serializer.Serialize(pCreator.GetProblem<PronounInstance>(), Path.Combine(args.OutDir, "PronounInstance.prb"), args.DoScale);
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(arg => arg.EMRDirs)
                .As('e', "emr")
                .Required();

            p.Setup(arg => arg.EMRFormat)
                .As('f', "emrformat")
                .SetDefault(EMRFormat.I2B2);

            p.Setup(arg => arg.Language)
                .As('l', "language")
                .SetDefault(Language.English);

            p.Setup(arg => arg.Mode)
                .As('m', "mode")
                .Required();

            p.Setup(arg => arg.OutDir)
                .As('o', "outdir")
                .Required();

            p.Setup(arg => arg.ClasMethod)
                .As('c', "clasmethod")
                .SetDefault(ClasMethod.LibSVM);

            p.Setup(arg => arg.Random)
                .As('r', "random")
                .SetDefault(0);

            p.Setup(arg => arg.ModelsDir)
                .As('d', "models")
                .SetDefault(null);

            p.Setup(a => a.DoScale)
                .As('s', "scale")
                .SetDefault(true);

            return p;
        }
    }
}
