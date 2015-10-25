using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public struct Evaluation
    {
        public static readonly ConceptType[] Types =
        {
            ConceptType.None,
            ConceptType.Person,
            ConceptType.Problem,
            ConceptType.Test,
            ConceptType.Treatment
        };

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
    }
}
