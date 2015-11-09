using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using HCMUT.EMRCorefResol.Core.Console;
using System.IO;
using HCMUT.EMRCorefResol.Scoring;

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
                var evals = Evaluations.Evaluate(emr, groundTruth, systemChains);

                var s = Evaluations.Stringify(evals);
                Console.WriteLine(s);
                File.WriteAllText(Path.Combine(args.OutputDir, $"{args.EMRFile}.scores"), s);
            }
            else
            {
                var emrCollection = new EMRCollection(args.EMRDir);
                var emrCount = emrCollection.Count;
                var evals = new IIndexedEnumerable<Dictionary<ConceptType, Evaluation>>[emrCount];

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
                        evals[i] = Evaluations.Evaluate(emr, groundTruth, systemChains);
                    }

                    File.WriteAllText(Path.Combine(args.OutputDir, $"{emrFile}.scores"),
                        Evaluations.Stringify(evals[i]));
                });

                var nMetrics = Evaluations.Metrics.Count;
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

                                foreach (var t in Evaluations.ConceptTypes)
                                {
                                    var e = eval[i].ContainsKey(t) ? eval[i][t] : new Evaluation(0d, 0d, 0d, Evaluations.Metrics[i].Name);

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
                    foreach (var t in Evaluations.ConceptTypes)
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

                foreach (var t in Evaluations.ConceptTypes)
                {
                    var e = avgEvals[nMetrics][t];
                    avgEvals[nMetrics][t] = new Evaluation(e.Precision / nMetrics,
                        e.Recall / nMetrics, e.FMeasure / nMetrics, e.MetricName);
                }

                var s = Evaluations.Stringify(avgEvals.ToIndexedEnumerable());
                Console.WriteLine("Average scores:");
                Console.WriteLine(s);
                File.WriteAllText(args.AverageFile, s);
            }
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
    }
}
