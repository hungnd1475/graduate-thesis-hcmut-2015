using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class TrainingResult
    {
        public long CompletionTime { get; }
        public IClassifier Classifier { get; }

        public TrainingResult(long completionTime, 
            IClassifier classifer)
        {
            CompletionTime = completionTime;
            Classifier = classifer;
        }
    }
}
