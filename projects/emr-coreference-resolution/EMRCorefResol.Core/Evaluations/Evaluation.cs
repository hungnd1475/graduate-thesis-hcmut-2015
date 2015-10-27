using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public struct Evaluation
    {
        public static readonly IIndexedEnumerable<ConceptType> ConceptTypes;
        public static readonly IIndexedEnumerable<IPerfMetric> Metrics;

        static Evaluation()
        {
            ConceptTypes = IndexedEnumerableHelper.ToIndexedEnumerable(new[]
            {
                ConceptType.None,
                ConceptType.Person,
                ConceptType.Problem,
                ConceptType.Test,
                ConceptType.Treatment
            });

            Metrics = IndexedEnumerableHelper.ToIndexedEnumerable(new IPerfMetric[]
            {
                new MUCPerfMetric(),
                new BCubedPerfMetric(),
                new CEAFPerfMetric()
            });
        }

        public double Precision { get; }
        public double Recall { get; }
        public double FMeasure { get; }
        public string MetricName { get; }

        public Evaluation(double precision, double recall, double fmeasure, string metricName)
            : this()
        {
            Precision = precision;
            Recall = recall;
            FMeasure = fmeasure;
            MetricName = metricName;
        }

        public static Evaluation Zero(string metricName)
        {
            return new Evaluation(0d, 0d, 0d, metricName);
        }
    }
}
