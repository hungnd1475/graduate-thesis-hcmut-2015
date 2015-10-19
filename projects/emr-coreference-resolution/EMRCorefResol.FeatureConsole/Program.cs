﻿using System;
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
            else if (parseResult.HelpCalled)
            {
                return;
            }

            var args = argsParser.Object;
            var emrCollections = args.EMRDirs.Split('|').Select(ep => new EMRCollection(ep));

            if (args.Mode != Mode.Classify && emrCollections.Any(e => !e.HasGroundTruth))
            {
                Console.Write("EMR paths must contain chains directories in Train or Test mode.");
                Console.ReadLine();
                return;
            }

            if (args.Mode != Mode.Train && !Directory.Exists(args.ModelsDir))
            {
                Console.Write("Models directory must be specified in Test or Classify mode.");
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
            var doScale = Convert.ToBoolean(args.DoScale);

            serializer.Serialize(pCreator.GetProblem<PersonPair>(), Path.Combine(args.OutDir, "PersonPair.prb"), doScale);
            serializer.Serialize(pCreator.GetProblem<PersonInstance>(), Path.Combine(args.OutDir, "PersonInstance.prb"), doScale);
            serializer.Serialize(pCreator.GetProblem<PronounInstance>(), Path.Combine(args.OutDir, "PronounInstance.prb"), doScale);
            serializer.Serialize(pCreator.GetProblem<ProblemPair>(), Path.Combine(args.OutDir, "ProblemPair.prb"), doScale);
            serializer.Serialize(pCreator.GetProblem<TreatmentPair>(), Path.Combine(args.OutDir, "TreatmentPair.prb"), doScale);
            serializer.Serialize(pCreator.GetProblem<TestPair>(), Path.Combine(args.OutDir, "TestPair.prb"), doScale);
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.SetupHelp("-?", "--help");
            p.HelpOption = new DescHelpOption();

            p.Setup(arg => arg.EMRDirs)
                .As('e', "emr")
                .Required()
                .WithDescription("Set a collection of EMR directories, separated by | (required).");

            p.Setup(arg => arg.EMRFormat)
                .As('f', "emrformat")
                .SetDefault(EMRFormat.I2B2)
                .WithDescription(Descriptions.EMRFormat("1"));

            p.Setup(arg => arg.Language)
                .As('l', "language")
                .SetDefault(Language.English)
                .WithDescription(Descriptions.Language("1"));

            p.Setup(arg => arg.Mode)
                .As('m', "mode")
                .Required()
                .WithDescription(Descriptions.Mode(null));

            p.Setup(arg => arg.OutDir)
                .As('o', "outdir")
                .Required()
                .WithDescription("Set output directory (required).");

            p.Setup(arg => arg.ClasMethod)
                .As('c', "clasmethod")
                .SetDefault(ClasMethod.LibSVM)
                .WithDescription(Descriptions.ClasMethod("1"));

            p.Setup(arg => arg.Random)
                .As('r', "random")
                .SetDefault(0)
                .WithDescription("Set the number of random chosen EMR files to extract (default all).");

            p.Setup(arg => arg.ModelsDir)
                .As('d', "models")
                .SetDefault(null)
                .WithDescription("Set the path to trained models directory (required in Test or Classify mode).");

            p.Setup(a => a.DoScale)
                .As('s', "scale")
                .SetDefault(1)
                .WithDescription("Whether the extractor should scale all features before saving (default 1).");

            return p;
        }
    }
}
