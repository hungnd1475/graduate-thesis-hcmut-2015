using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Classification;

namespace HCMUT.EMRCorefResol.ResolvingConsole
{
    class Program
    {
        static void Main(string[] rawArgs)
        {
            var parser = PrepareParser();
            var result = parser.Parse(rawArgs);

            if (result.HasErrors)
            {
                DescHelpOption.ShowHelp(parser.Options);
                return;
            }
            else if (result.HelpCalled)
            {
                return;
            }

            var args = parser.Object;

            if (!Directory.Exists(args.EMRDir))
            {
                Console.WriteLine("Cannot find EMR directory.");
                return;
            }

            if (!Directory.Exists(args.ModelsDir))
            {
                Console.WriteLine("Cannot find classification models directory.");
                return;
            }

            var dataReader = APISelector.SelectDataReader(args.EMRFormat);
            var classifier = APISelector.SelectClassifier(args.ClasMethod, args.ModelsDir);
            var fExtractor = APISelector.SelectFeatureExtractor(args.Language, Mode.Classify, args.ClasMethod, args.ModelsDir);
            var resolver = APISelector.SelectResolver(args.ResolMethod);

            if (!string.IsNullOrEmpty(args.EMRName))
            {
                Console.WriteLine($"Resolving {args.EMRName}.txt...");
                var dir = args.EMRDir;
                var emrFile = Path.Combine(dir, "docs", $"{args.EMRName}.txt");
                var conceptsFile = Path.Combine(dir, "concepts", $"{args.EMRName}.txt.con");
                var chains = Resolve(emrFile, conceptsFile, dataReader, resolver, fExtractor, classifier);
                File.WriteAllText(Path.Combine(args.OutputDir, $"{args.EMRName}.txt.chains"), string.Join(Environment.NewLine, chains));
                Console.WriteLine("Done.");
            }
            else
            {
                var emrCollection = new EMRCollection(args.EMRDir);
                for (int i = 0; i < emrCollection.Count; i++)
                {
                    var emrFile = emrCollection.GetEMRPath(i);
                    var emrName = Path.GetFileName(emrFile);
                    Console.WriteLine($"Resolving {emrName}...");

                    var conceptsFile = emrCollection.GetConceptsPath(i);
                    var chains = Resolve(emrFile, conceptsFile, dataReader, resolver, fExtractor, classifier);
                    File.WriteAllText(Path.Combine(args.OutputDir, $"{emrName}.chains"), 
                        string.Join(Environment.NewLine, chains));
                    Console.WriteLine("Done.");

                    //string ans = string.Empty;
                    //while (true)
                    //{
                    //    Console.Write("Do you want to resolve next emr? (Y/N) ");
                    //    ans = Console.ReadLine().ToLower();
                    //    if (ans.Equals("n") || ans.Equals("y"))
                    //        break;
                    //}

                    //if (ans.Equals("n"))
                    //    break;
                }
            }
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.EMRDir)
                .As('d', "emrdir")
                .Required()
                .WithDescription("Set EMR directory (required).");

            p.Setup(a => a.EMRName)
                .As('e', "emrname")
                .SetDefault(null)
                .WithDescription("Set specific EMR file name (without extension) to resolve (default all).");

            p.Setup(a => a.EMRFormat)
                .As('f', "emrformat")
                .SetDefault(EMRFormat.I2B2)
                .WithDescription(Descriptions.EMRFormat("1"));

            p.Setup(a => a.Language)
                .As('l', "language")
                .SetDefault(Language.English)
                .WithDescription(Descriptions.Language("1"));

            p.Setup(a => a.ModelsDir)
                .As('m', "models")
                .Required()
                .WithDescription("Set classification models directory (required).");

            p.Setup(a => a.ClasMethod)
                .As('c', "clas")
                .SetDefault(ClasMethod.LibSVM)
                .WithDescription(Descriptions.ClasMethod("1"));

            p.Setup(a => a.ResolMethod)
                .As('r', "resol")
                .SetDefault(ResolMethod.BestFirst)
                .WithDescription(Descriptions.ResolMethod("1"));

            p.Setup(a => a.OutputDir)
                .As('o', "outdir")
                .Required()
                .WithDescription("Set output directory (required).");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }

        static CorefChainCollection Resolve(string emrFile, string conceptsFile, IDataReader dataReader,
            ICorefResolver resolver, IFeatureExtractor fExtractor, IClassifier classifier)
        {
            var emr = new EMR(emrFile, conceptsFile, dataReader);
            return resolver.Resolve(emr, fExtractor, classifier);
        }
    }
}
