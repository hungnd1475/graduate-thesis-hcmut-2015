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

            if (!Directory.Exists(args.EMRDir))
            {
                Console.WriteLine("Ground truth directory not found.");
                return;
            }

            if (!Directory.Exists(args.SystemChainsDir))
            {
                Console.WriteLine("System chains directory not found.");
                return;
            }

            var dataReader = APISelector.SelectDataReader(args.Format);

            if (!string.IsNullOrEmpty(args.EMRFile))
            {
                var chainsFile = $"{args.EMRFile}.chains";
                var gtFile = Path.Combine(args.EMRDir, "chains", chainsFile);
                var scFile = Path.Combine(args.SystemChainsDir, chainsFile);

                if (!File.Exists(gtFile))
                {
                    Console.WriteLine($"Ground truth file for {args.EMRFile} not found.");
                    return;
                }
                else if (!File.Exists(scFile))
                {
                    Console.WriteLine($"System chains file for {args.EMRFile} not found.");
                    return;
                }

                Console.WriteLine($"Evaluating {args.EMRFile}...");

                var emrFile = Path.Combine(args.EMRDir, "docs", args.EMRFile);
                var conceptsFile = Path.Combine(args.EMRDir, "concepts", $"{args.EMRFile}.con");
                var emr = new EMR(emrFile, conceptsFile, dataReader);
                var groundTruth = new CorefChainCollection(gtFile, dataReader);
                var systemChains = new CorefChainCollection(scFile, dataReader);
                var evals = Evaluation.Metrics.Select(m => m.Evaluate(emr, groundTruth, systemChains)).ToArray();

                var s = StringifyScores(evals);
                Console.WriteLine(s);
                File.WriteAllText(Path.Combine(args.OutputDir, $"{args.EMRFile}.scores"), s);
            }
            else
            {
                var emrCollection = new EMRCollection(args.EMRDir);
                var emrCount = emrCollection.Count;
                var evals = new Dictionary<ConceptType, Evaluation>[emrCount][];

                Parallel.For(0, emrCount, i =>
                {
                    var gtPath = emrCollection.GetChainsPath(i);
                    var gtFile = Path.GetFileName(gtPath);
                    var scPath = Path.Combine(args.SystemChainsDir, gtFile);

                    var emrPath = emrCollection.GetEMRPath(i);
                    var emrFile = Path.GetFileName(emrPath);
                    Console.WriteLine($"Evaluating {emrFile}...");

                    if (!File.Exists(scPath))
                    {
                        Console.WriteLine("System chains file not found.");
                        evals[i] = null;
                    }
                    else
                    {
                        var emr = new EMR(emrPath, emrCollection.GetConceptsPath(i), dataReader);
                        var groundTruth = new CorefChainCollection(gtPath, dataReader);
                        var systemChains = new CorefChainCollection(scPath, dataReader);
                        evals[i] = Evaluation.Metrics.Select(m => m.Evaluate(emr, groundTruth, systemChains)).ToArray();
                    }

                    File.WriteAllText(Path.Combine(args.OutputDir, $"{emrFile}.scores"),
                        StringifyScores(evals[i]));
                });

                var nMetrics = Evaluation.Metrics.Count;
                var avgEvals = evals.Aggregate(new Dictionary<ConceptType, Evaluation>[nMetrics + 1],
                    (avg, eval) =>
                    {
                        if (eval != null)
                        {
                            for (int i = 0; i < nMetrics; i++)
                            {
                                if (avg[i] == null)
                                {
                                    avg[i] = new Dictionary<ConceptType, Evaluation>();
                                }

                                foreach (var t in Evaluation.ConceptTypes)
                                {
                                    var e = eval[i].ContainsKey(t) ? eval[i][t] : new Evaluation(0d, 0d, 0d, Evaluation.Metrics[i].Name);

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
                        }
                        return avg;
                    });

                avgEvals[nMetrics] = new Dictionary<ConceptType, Evaluation>();
                for (int i = 0; i < nMetrics; i++)
                {
                    foreach (var t in Evaluation.ConceptTypes)
                    {
                        var a = avgEvals[i][t];
                        avgEvals[i][t] = new Evaluation(a.Precision / emrCount,
                            a.Recall / emrCount, a.FMeasure / emrCount, a.MetricName);

                        a = avgEvals[i][t];
                        var e = avgEvals[nMetrics].ContainsKey(t) ? avgEvals[nMetrics][t] : new Evaluation(0d, 0d, 0d, "Average");
                        avgEvals[nMetrics][t] = new Evaluation(a.Precision + e.Precision,
                            a.Recall + e.Recall, a.FMeasure + e.FMeasure, e.MetricName);
                    }
                }

                foreach (var t in Evaluation.ConceptTypes)
                {
                    var e = avgEvals[nMetrics][t];
                    avgEvals[nMetrics][t] = new Evaluation(e.Precision / nMetrics,
                        e.Recall / nMetrics, e.FMeasure / nMetrics, e.MetricName);
                }

                var s = StringifyScores(avgEvals);
                Console.WriteLine("Average scores:");
                Console.WriteLine(s);
                File.WriteAllText(args.AverageFile, s);
            }
        }

        static Dictionary<ConceptType, Evaluation>[] ZeroEvaluations()
        {
            var evals = new Dictionary<ConceptType, Evaluation>[Evaluation.Metrics.Count];
            for (int i = 0; i < Evaluation.Metrics.Count; i++)
            {
                foreach (var t in Evaluation.ConceptTypes)
                {
                    evals[i] = new Dictionary<ConceptType, Evaluation>();
                    evals[i][t] = Evaluation.Zero(Evaluation.Metrics[i].Name);
                }
            }
            return evals;
        }

        static FluentCommandLineParser<Arguments> PrepareParser()
        {
            var p = new FluentCommandLineParser<Arguments>();

            p.Setup(a => a.EMRDir)
                .As('d', "emrdir")
                .Required()
                .WithDescription("Set EMR directory (required).");

            p.Setup(a => a.SystemChainsDir)
                .As('s', "systemchains")
                .Required()
                .WithDescription("Set system chains directory (required).");

            p.Setup(a => a.Format)
                .As('f', "format")
                .SetDefault(EMRFormat.I2B2)
                .WithDescription(Descriptions.EMRFormat("1"));

            p.Setup(a => a.OutputDir)
                .As('o', "outdir")
                .Required()
                .WithDescription("Set output file path (required).");

            p.Setup(a => a.AverageFile)
                .As('a', "avgfile")
                .SetDefault(null)
                .WithDescription("Set score file path (required if many EMRs to be resolved).");

            p.Setup(a => a.EMRFile)
                .As('e', "emr")
                .SetDefault(null)
                .WithDescription("Set emr file name to evaluate (optional).");

            p.SetupHelp("?").Callback(() => DescHelpOption.ShowHelp(p.Options));

            return p;
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

            foreach (var type in Evaluation.ConceptTypes)
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
    }
}
