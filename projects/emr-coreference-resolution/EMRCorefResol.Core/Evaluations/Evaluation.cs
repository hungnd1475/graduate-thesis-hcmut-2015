using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public struct Evaluation
    {
        public static readonly Evaluation Zero = new Evaluation(0d, 0d, 0d);

        public double Precision { get; }
        public double Recall { get; }
        public double FMeasure { get; }

        public Evaluation(double precision, double recall, double fmeasure)
            : this()
        {
            Precision = precision;
            Recall = recall;
            FMeasure = fmeasure;
        }
    }
}
