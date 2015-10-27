using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Evaluations;

namespace HCMUT.EMRCorefResol.ResolvingConsole
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
            var fExtractor = APISelector.SelectFeatureExtractor(args.Language, Mode.Classify, args.ClasMethod, args.ModelsDir);
            var resolver = APISelector.SelectResolver(args.ResolMethod);

            if (!string.IsNullOrEmpty(args.EMRName))
            {
                Console.WriteLine($"Resolving {args.EMRName}...");
                var dir = args.EMRDirs;
                var emrFile = Path.Combine(dir, "docs", $"{args.EMRName}");
                var conceptsFile = Path.Combine(dir, "concepts", $"{args.EMRName}.con");
                var chainsFile = Path.Combine(dir, "chains", $"{args.EMRName}.chains");

                var result = ResolveAndWrite(emrFile, conceptsFile, chainsFile, dataReader,
                    resolver, fExtractor, classifier, Path.Combine(args.OutputDir, $"{args.EMRName}.chains"),
                    Path.Combine(args.OutputDir, $"{args.EMRName}.scores"));

                if (result.Evaluations != null)
                {
                    var scores = StringifyScores(result.Evaluations);
                    Console.WriteLine(scores);
                }

                Console.WriteLine("Done!");
            }
            else
            {
                if (string.IsNullOrEmpty(args.ScoresFile))
                {
                    Console.WriteLine("Score file must be set if there are many EMRs to be resolved.");
                    return;
                }

                var emrCollection = new EMRCollection(args.EMRDirs);
                var emrCount = args.EMRCount > 0 ? args.EMRCount : emrCollection.Count;
                var evals = new Dictionary<ConceptType, Evaluation>[emrCount][];

                for (int i = 0; i < emrCount && i < emrCollection.Count; i++)
                {
                    var emrFile = emrCollection.GetEMRPath(i);
                    var emrName = Path.GetFileName(emrFile);

                    Console.WriteLine($"Resolving {emrName}...");

                    var conceptsFile = emrCollection.GetConceptsPath(i);
                    var chainsFile = emrCollection.GetChainsPath(i);

                    var result = ResolveAndWrite(emrFile, conceptsFile, chainsFile, dataReader,
                        resolver, fExtractor, classifier, Path.Combine(args.OutputDir, $"{emrName}.chains"),
                        Path.Combine(args.OutputDir, $"{emrName}.scores"));

                    evals[i] = result.Evaluations;
                };

                Console.WriteLine("Calculating average...");
                var avgEvals = evals.Aggregate(new Dictionary<ConceptType, Evaluation>[PerfMetrics.Length + 1],
                    (avg, eval) =>
                    {
                        for (int i = 0; i < PerfMetrics.Length; i++)
                        {
                            if (avg[i] == null)
                            {
                                avg[i] = new Dictionary<ConceptType, Evaluation>();
                            }

                            foreach (var t in Types)
                            {
                                var e = eval[i].ContainsKey(t) ? eval[i][t] : new Evaluation(0d, 0d, 0d, PerfMetrics[i].Name);

                                if (!avg[i].ContainsKey(t))
                                {                                    
                                    var ep = double.IsNaN(e.Precision) ? 0 : e.Precision;
                                    var er = double.IsNaN(e.Recall) ? 0 : e.Recall;
                                    var ef = double.IsNaN(e.FMeasure) ? 0 : e.FMeasure;
                                    avg[i].Add(t, new Evaluation(ep, er, ef, e.MetricName));
                                }
                                else
                                {
                                    var a = avg[i][t];

                                    var ep = double.IsNaN(e.Precision) ? 0 : e.Precision;
                                    var er = double.IsNaN(e.Recall) ? 0 : e.Recall;
                                    var ef = double.IsNaN(e.FMeasure) ? 0 : e.FMeasure;

                                    avg[i][t] = new Evaluation(a.Precision + ep,
                                        a.Recall + er, a.FMeasure + ef, e.MetricName);
                                }
                            }
                        }
                        return avg;
                    });

                var mLength = PerfMetrics.Length;
                avgEvals[mLength] = new Dictionary<ConceptType, Evaluation>();

                for (int i = 0; i < PerfMetrics.Length; i++)
                {
                    foreach (var t in Types)
                    {
                        var a = avgEvals[i][t];
                        avgEvals[i][t] = new Evaluation(a.Precision / emrCount,
                            a.Recall / emrCount, a.FMeasure / emrCount, a.MetricName);

                        a = avgEvals[i][t];
                        var e = avgEvals[mLength].ContainsKey(t) ? avgEvals[mLength][t] : new Evaluation(0d, 0d, 0d, "Average");
                        avgEvals[mLength][t] = new Evaluation(a.Precision + e.Precision,
                            a.Recall + e.Recall, a.FMeasure + e.FMeasure, e.MetricName);
                    }
                }

                foreach (var t in Types)
                {
                    var e = avgEvals[mLength][t];
                    avgEvals[mLength][t] = new Evaluation(e.Precision / mLength,
                        e.Recall / mLength, e.FMeasure / mLength, e.MetricName);                    
                }

                File.WriteAllText(args.ScoresFile, StringifyScores(avgEvals));
                Console.WriteLine("Done!");
            }
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

            p.Setup(a => a.ScoresFile)
                .As('s', "score")
                .SetDefault(null)
                .WithDescription("Set score file path (required if many EMRs to be resolved).");

            p.Setup(a => a.EMRCount)
                .As('n', "count")
                .SetDefault(0)
                .WithDescription("Set number of emr to resolve (optional).");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
        }

        static Resolution ResolveAndWrite(string emrFile, string conceptsFile, string chainsFile, IDataReader dataReader,
            ICorefResolver resolver, IFeatureExtractor fExtractor, IClassifier classifier, string outputFile,
            string scoresFile)
        {
            var emr = new EMR(emrFile, conceptsFile, dataReader);
            var systemChains = resolver.Resolve(emr, fExtractor, classifier);
            File.WriteAllText(outputFile, string.Join(Environment.NewLine, systemChains));

            Console.WriteLine("Evaluating...");
            Dictionary<ConceptType, Evaluation>[] evals = null;

            if (File.Exists(chainsFile))
            {
                var groundTruth = new CorefChainCollection(chainsFile, dataReader);
                evals = PerfMetrics.Select(m => m.Evaluate(emr, groundTruth, systemChains)).ToArray();
                File.WriteAllText(scoresFile, StringifyScores(evals));
            }
            else
            {
                Console.WriteLine("Chains file not found.");
            }

            return new Resolution(systemChains, evals);
        }

        static string StringifyScores(Dictionary<ConceptType, Evaluation>[] scores)
        {
            var sb = new StringBuilder();
            var metrics = scores.Select(s => s[ConceptType.None].MetricName).ToArray();

            sb.Append('-', 11);
            for (int i = 0; i < metrics.Length; i++)
            {
                sb.Append('-', 32);
            }

            sb.AppendLine();
            sb.Append(' ', 11);
            foreach (var m in metrics)
            {
                sb.Append($"{m,-32}");
            }

            sb.AppendLine();
            sb.Append(' ', 11);
            for (int i = 0; i < metrics.Length; i++)
            {
                sb.Append('-', 30);
                sb.Append(' ', 2);
            }

            sb.AppendLine();
            sb.Append(' ', 11);
            for (int i = 0; i < metrics.Length; i++)
            {
                sb.Append($"{"P",-10}{"R",-10}{"F",-10}  ");
            }

            sb.AppendLine();
            sb.Append('-', 11);
            for (int i = 0; i < metrics.Length; i++)
            {
                sb.Append('-', 32);
            }

            foreach (var type in Types)
            {
                sb.AppendLine();

                var name = type == ConceptType.None ? "All" : type.ToString();
                sb.Append($"{name,-11}");

                foreach (var evals in scores)
                {
                    double p = 0d, r = 0d, f = 0d;

                    if (evals.ContainsKey(type))
                    {
                        p = evals[type].Precision;
                        r = evals[type].Recall;
                        f = evals[type].FMeasure;
                    }

                    sb.Append($"{p,-10:N3}");
                    sb.Append($"{r,-10:N3}");
                    sb.Append($"{f,-10:N3}");
                    sb.Append(' ', 2);
                }
            }

            sb.AppendLine();
            sb.Append('-', 11);
            for (int i = 0; i < metrics.Length; i++)
            {
                sb.Append('-', 32);
            }

            return sb.ToString();
        }

        struct Resolution
        {
            public CorefChainCollection Chains { get; }
            public Dictionary<ConceptType, Evaluation>[] Evaluations { get; }

            public Resolution(CorefChainCollection chains)
                : this(chains, null)
            {
            }

            public Resolution(CorefChainCollection chains, Dictionary<ConceptType, Evaluation>[] evaluations)
                : this()
            {
                Chains = chains;
                Evaluations = evaluations;
            }
        }
    }
}
