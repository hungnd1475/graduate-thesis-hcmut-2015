using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Training.English
{
    public class EnglishTrainingFeatureExtractor : IFeatureExtractor
    {
        public IFeatureVector Extract(TreatmentPair instance, string emr)
        {
            throw new NotImplementedException();
        }

        public IFeatureVector Extract(SingleInstance instance, string emr)
        {
            throw new NotImplementedException();
        }

        public IFeatureVector Extract(TestPair instance, string emr)
        {
            throw new NotImplementedException();
        }

        public IFeatureVector Extract(ProblemPair instance, string emr)
        {
            throw new NotImplementedException();
        }

        public IFeatureVector Extract(PersonPair instance, string emr)
        {
            throw new NotImplementedException();
        }
    }
}
