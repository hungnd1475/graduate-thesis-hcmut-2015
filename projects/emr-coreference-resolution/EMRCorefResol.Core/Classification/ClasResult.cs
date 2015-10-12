using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public class ClasResult
    {
        public double Class { get; }

        public double Confidence { get; }

        public ClasResult(double _class, double confidence)
        {
            Class = _class;
            Confidence = confidence;
        }
    }
}
