using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Scoring
{
    public static class Evaluations
    {
        public static readonly IIndexedEnumerable<ConceptType> ConceptTypes =
            new[]
            {
                ConceptType.None,
                ConceptType.Person,
                ConceptType.Problem,
                ConceptType.Test,
                ConceptType.Treatment
            }
            .ToIndexedEnumerable();

        public static readonly IIndexedEnumerable<IPerfMetric> Metrics =
            new IPerfMetric[]
            {
                new MUCPerfMetric(),
                new BCubedPerfMetric(),
                new CEAFPerfMetric()
            }
            .ToIndexedEnumerable();

        public static IIndexedEnumerable<Dictionary<ConceptType, Evaluation>> Evaluate(
            EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            return Metrics.Select(m => m.Evaluate(emr, groundTruth, systemChains)).ToIndexedEnumerable();
        }

        public static string Stringify(IIndexedEnumerable<Dictionary<ConceptType, Evaluation>> scores)
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

            foreach (var type in ConceptTypes)
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
