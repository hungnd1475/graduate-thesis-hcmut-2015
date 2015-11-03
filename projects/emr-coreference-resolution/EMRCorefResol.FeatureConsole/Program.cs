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
                DescHelpOption.ShowHelp(argsParser.Options);
                return;
            }
            else if (parseResult.HelpCalled)
            {
                return;
            }

            var args = argsParser.Object;
            var emrCollection = new EMRCollection(args.EMRDir);

            if (args.Mode != Mode.Classify && !emrCollection.HasGroundTruth)
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

            var types = args.Instances?.Split(',').Aggregate(new HashSet<Type>(), 
                (t, s) =>
                {
                    Instance e;
                    if (Enum.TryParse(s.Trim(), out e))
                    {
                        t.Add(GetTypeFromEnum(e));
                    }
                    return t;
                });

            var pCreator = new ClasProblemCollection();
            var dataReader = APISelector.SelectDataReader(args.EMRFormat);
            var fExtractor = APISelector.SelectFeatureExtractor(args.Language, args.Mode, 
                args.ClasMethod, args.ModelsDir);
            var instancesGenerator = new AllInstancesGenerator();

            if (!string.IsNullOrEmpty(args.EMRFile))
            {
                var index = emrCollection.IndexOf(args.EMRFile);
                var emrPath = emrCollection.GetEMRPath(index);
                var conceptsPath = emrCollection.GetConceptsPath(index);
                var chainsPath = emrCollection.GetChainsPath(index);
                FeatureExtractingSystem.Instance.ExtractOne(emrPath, conceptsPath, chainsPath, dataReader,
                    instancesGenerator, fExtractor, pCreator, null, types);
            }
            else if (args.Random > 0)
            {
                // TODO: refine this
                string[] emrPaths, conceptsPaths, chainsPaths;
                emrCollection.GetRandom(args.Random, out emrPaths, out conceptsPaths, out chainsPaths);
                FeatureExtractingSystem.Instance.ExtractAll(emrPaths, conceptsPaths, chainsPaths, dataReader,
                    instancesGenerator, fExtractor, pCreator, null, types);
            }
            else
            {
                FeatureExtractingSystem.Instance.ExtractCollection(emrCollection, dataReader, instancesGenerator,
                    fExtractor, pCreator, null, types);
            }

            var serializer = APISelector.SelectProblemSerializer(args.ClasMethod);
            Directory.CreateDirectory(args.OutputDir);

            Console.WriteLine("Serializing...");
            var doScale = Convert.ToBoolean(args.DoScale);

            if (types == null || types.Count == 0)
            {
                serializer.Serialize(pCreator.GetProblem<PersonPair>(), Path.Combine(args.OutputDir, "PersonPair.prb"), doScale);
                serializer.Serialize(pCreator.GetProblem<PersonInstance>(), Path.Combine(args.OutputDir, "PersonInstance.prb"), doScale);
                serializer.Serialize(pCreator.GetProblem<PronounInstance>(), Path.Combine(args.OutputDir, "PronounInstance.prb"), doScale);
                serializer.Serialize(pCreator.GetProblem<ProblemPair>(), Path.Combine(args.OutputDir, "ProblemPair.prb"), doScale);
                serializer.Serialize(pCreator.GetProblem<TreatmentPair>(), Path.Combine(args.OutputDir, "TreatmentPair.prb"), doScale);
                serializer.Serialize(pCreator.GetProblem<TestPair>(), Path.Combine(args.OutputDir, "TestPair.prb"), doScale);
            }
            else
            {
                foreach (var t in types)
                {
                    serializer.Serialize(pCreator.GetProblem(t), Path.Combine(args.OutputDir, $"{t.Name}.prb"), doScale);
                }
            }
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(arg => arg.EMRDir)
                .As('d', "emrdir")
                .Required()
                .WithDescription("Set a collection of EMR directory.");

            p.Setup(a => a.EMRFile)
                .As('e', "emrfile")
                .SetDefault(null)
                .WithDescription("Set emr file name to extract (optional).");

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

            p.Setup(arg => arg.OutputDir)
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
                .As('t', "models")
                .SetDefault(null)
                .WithDescription("Set the path to trained models directory (required in Test or Classify mode).");

            p.Setup(a => a.DoScale)
                .As('s', "scale")
                .SetDefault(1)
                .WithDescription("Whether the extractor should scale all features before saving (default 1).");

            p.Setup(a => a.Instances)
                .As('i', "instances")
                .SetDefault(null)
                .WithDescription("Set intance types to extract.");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }

        static Type GetTypeFromEnum(Instance instance)
        {
            switch (instance)
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
                    throw new ArgumentException("instance");
            }
        }
    }
}
