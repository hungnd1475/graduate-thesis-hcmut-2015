using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.IO;
using HCMUT.EMRCorefResol.English;
using System.IO;
using HCMUT.EMRCorefResol.Classification.LibSVM;

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
            var emrCollections = argsResult.EMRPaths.Select(ep => new EMRCollection(ep));

            if (argsResult.Mode != Mode.Classify && emrCollections.Any(e => !e.HasGroundTruth))
            {
                Console.Write("EMR paths must contain chains directories in Train or Test mode");
                Console.ReadLine();
                return;
            }

            if (argsResult.Mode != Mode.Train && !File.Exists(argsResult.ModelPath))
            {
                Console.Write("Model file must be specified in Test or Classify mode.");
                Console.ReadLine();
                return;
            }

            var pCreator = new ClasProblemCreator();
            var dataReader = CreateDataReader(argsResult);
            var fExtractor = CreateFeatureExtractor(argsResult);
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

            var serializer = GetProbSerializer(argsResult);
            serializer.Serialize(pCreator.GetProblem<PersonPair>(), Path.Combine(argsResult.OutDir, "PersonPair.prb"));
            serializer.Serialize(pCreator.GetProblem<PersonInstance>(), Path.Combine(argsResult.OutDir, "PersonInstance.prb"));
            serializer.Serialize(pCreator.GetProblem<PronounInstance>(), Path.Combine(argsResult.OutDir, "PronounInstance.prb"));
        }

        static FluentCommandLineParser<Arguments> PrepareArgsParser(string[] args)
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(arg => arg.EMRPaths)
                .As('e', "emr")
                .Required();

            p.Setup(arg => arg.Language)
                .As('l', "language")
                .SetDefault(Language.English);

            p.Setup(arg => arg.Mode)
                .As('m', "mode")
                .Required();

            p.Setup(arg => arg.OutDir)
                .As('o', "outdir")
                .Required();

            p.Setup(arg => arg.OutputFormat)
                .As('f', "outformat")
                .SetDefault(OutputFormat.LibSVM);

            p.Setup(arg => arg.Random)
                .As('r', "random")
                .SetDefault(0);

            p.Setup(arg => arg.ModelPath)
                .As('d', "model")
                .SetDefault(null);

            return p;
        }

        static IDataReader CreateDataReader(Arguments args)
        {
            switch (args.DataFormat)
            {
                case DataFormat.I2B2:
                    return new I2B2DataReader();
                default:
                    throw new ArgumentException("Cannot create an instance of IDataReader based on dataFormat value.");
            }
        }

        static IFeatureExtractor CreateFeatureExtractor(Arguments args)
        {
            if (args.Mode == Mode.Train)
            {
                switch (args.Language)
                {
                    case Language.English:
                        return new EnglishTrainingFeatureExtractor();
                    case Language.Vietnamese:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                switch (args.Language)
                {
                    case Language.English:
                        return new EnglishClasFeatureExtractor(ClassifierSerializer.Deserialize(args.ModelPath));
                    case Language.Vietnamese:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException();
                }
            }
        }

        static IClasProblemSerializer GetProbSerializer(Arguments args)
        {
            switch (args.OutputFormat)
            {
                case OutputFormat.LibSVM:
                    return LibSVMProblemSerializer.Instance;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
