using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Scoring;
using System.Diagnostics;

namespace HCMUT.EMRCorefResol.ResolvingConsole
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

            if (!Directory.Exists(args.EMRDirs))
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
            var fExtractor = APISelector.SelectFeatureExtractor(args.Language, classifier);
            var resolver = APISelector.SelectResolver(args.ResolMethod);
            var shutdown = Convert.ToBoolean(args.Shutdown);

            if (!string.IsNullOrEmpty(args.EMRName))
            {
                Console.WriteLine($"Resolving {args.EMRName}...");
                var dir = args.EMRDirs;
                var emrFile = Path.Combine(dir, "docs", $"{args.EMRName}");
                var conceptsFile = Path.Combine(dir, "concepts", $"{args.EMRName}.con");

                var emr = new EMR(emrFile, conceptsFile, dataReader);
                var systemChains = resolver.Resolve(emr, fExtractor, classifier);
                File.WriteAllText(Path.Combine(args.OutputDir, $"{args.EMRName}.chains"),
                    string.Join(Environment.NewLine, systemChains));
            }
            else
            {
                var emrCollection = new EMRCollection(args.EMRDirs);
                int beginIndex = 0;

                if (!string.IsNullOrEmpty(args.BeginAt))
                {
                    beginIndex = emrCollection.IndexOf(args.BeginAt);
                    if (beginIndex < 0)
                    {
                        Console.WriteLine($"Begining EMR file {args.BeginAt} not found.");
                        return;
                    }
                }

                int emrCount = args.EMRCount > 0 ? args.EMRCount + beginIndex : emrCollection.Count;

                var startTime = DateTime.Now;
                Console.WriteLine($"Start at {startTime:g}");
                
                for (int i = beginIndex; i < emrCount && i < emrCollection.Count; i++)
                {
                    var emrFile = emrCollection.GetEMRPath(i);                    
                    var emrName = Path.GetFileName(emrFile);

                    Console.WriteLine($"Resolving {emrName} ({i}/{emrCollection.Count})...");
                    var conceptsFile = emrCollection.GetConceptsPath(i);
                    var emr = new EMR(emrFile, conceptsFile, dataReader);

                    var systemChains = resolver.Resolve(emr, fExtractor, classifier);
                    File.WriteAllText(Path.Combine(args.OutputDir, $"{emrName}.chains"), 
                        string.Join(Environment.NewLine, systemChains));
                };

                var endTime = DateTime.Now;
                Console.WriteLine($"End at {endTime:g}");
                Console.WriteLine($"Total time: {(endTime - startTime).TotalSeconds}s");

                if (shutdown)
                {
                    using (var sw = new StreamWriter(Path.Combine(args.OutputDir, "shutdown-log.txt")))
                    {
                        sw.WriteLine($"Start at {startTime:g}");
                        sw.WriteLine($"End at {endTime:g}");
                        sw.WriteLine($"Total time: {(endTime - startTime).TotalSeconds}s");
                    }

                    Process.Start("shutdown", "/s /t 0");
                }
            }

            Console.WriteLine("Done!");
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.EMRDirs)
                .As('d', "emrdirs")
                .Required()
                .WithDescription("Set EMR directory (required).");

            p.Setup(a => a.EMRName)
                .As('e', "emrname")
                .SetDefault(null)
                .WithDescription("Set specific EMR file name (with extension) to resolve (optional).");

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

            p.Setup(a => a.EMRCount)
                .As('n', "count")
                .SetDefault(0)
                .WithDescription("Set number of emr to resolve (optional).");

            p.Setup(a => a.BeginAt)
                .As('b', "beginat")
                .SetDefault(null)
                .WithDescription("Set emr file name to begin with (optional).");

            p.Setup(a => a.Shutdown)
                .As('s', "shutdown")
                .SetDefault(0)
                .WithDescription("Set whether to shutdown the computer after resolving (default 0)");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }
    }
}
