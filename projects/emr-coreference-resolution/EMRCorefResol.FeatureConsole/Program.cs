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
        static void Main(string[] args)
        {
            var argsParser = PrepareArgsParser(args);
            var parseResult = argsParser.Parse(args);

            if (parseResult.HasErrors)
            {
                Console.Write(parseResult.ErrorText);
                Console.ReadLine();
                return;
            }

            var argsResult = argsParser.Object;
            var emrCollections = argsResult.EMRDirs.Split(';').Select(ep => new EMRCollection(ep));

            if (argsResult.Mode != Mode.Classify && emrCollections.Any(e => !e.HasGroundTruth))
            {
                Console.Write("EMR paths must contain chains directories in Train or Test mode");
                Console.ReadLine();
                return;
            }

            if (argsResult.Mode != Mode.Train && !Directory.Exists(argsResult.ModelsDir))
            {
                Console.Write("Model file must be specified in Test or Classify mode.");
                Console.ReadLine();
                return;
            }

            var pCreator = new ClasProblemCollection();
            var dataReader = APISelector.SelectDataReader(argsResult.EMRFormat);
            var fExtractor = APISelector.SelectFeatureExtractor(argsResult.Language, argsResult.Mode, 
                argsResult.ClasMethod, argsResult.ModelsDir);
            var preprocessor = new SimplePreprocessor();

            if (argsResult.Random > 0)
            {
                // TODO: refine this
                string[] emrPaths, conceptsPaths, chainsPaths;
                emrCollections.First().GetRandom(argsResult.Random, out emrPaths, out conceptsPaths, out chainsPaths);
                FeatureExtractingSystem.Instance.ExtractAll(emrPaths, conceptsPaths, chainsPaths, dataReader,
                    preprocessor, fExtractor, pCreator);
            }
            else
            {
                FeatureExtractingSystem.Instance.ExtractCollections(emrCollections, dataReader, preprocessor,
                    fExtractor, pCreator);
            }

            var serializer = APISelector.SelectProblemSerializer(argsResult.ClasMethod);
            Directory.CreateDirectory(argsResult.OutDir);

            serializer.Serialize(pCreator.GetProblem<PersonPair>(), Path.Combine(argsResult.OutDir, "PersonPair.prb"));
            serializer.Serialize(pCreator.GetProblem<PersonInstance>(), Path.Combine(argsResult.OutDir, "PersonInstance.prb"));
            serializer.Serialize(pCreator.GetProblem<PronounInstance>(), Path.Combine(argsResult.OutDir, "PronounInstance.prb"));
        }

        static FluentCommandLineParser<Arguments> PrepareArgsParser(string[] args)
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

            return p;
        }
    }
}
