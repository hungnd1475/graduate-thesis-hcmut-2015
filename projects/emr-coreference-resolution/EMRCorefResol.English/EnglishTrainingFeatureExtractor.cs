﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English
{
    public class EnglishTrainingFeatureExtractor : ITrainingFeatureExtractor
    {
        public EMR EMR { get; set; }
        public CorefChainCollection GroundTruth { get; set; }

        public EnglishTrainingFeatureExtractor(EMR emr, CorefChainCollection groundTruth)
        {
            EMR = emr;
            GroundTruth = groundTruth;
        }

        public IFeatureVector Extract(PronounInstance instance)
        {
            var corefChain = GroundTruth.FindChainContains(instance.Concept);
            var classValue = 0.0;
            if (corefChain!=null)
            {
                classValue = (double)corefChain.Type;
            }
            return new PronounFeatures(instance, EMR, GroundTruth, classValue); ;
        }

        public IFeatureVector Extract(PersonInstance instance)
        {
            return null;
        }

        public IFeatureVector Extract(TreatmentPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(TestPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(ProblemPair instance)
        {
            return null;
        }

        public IFeatureVector Extract(PersonPair instance)
        {
            var classValue = GroundTruth.IsCoref(instance) ? 1.0 : 0.0;
            return new PersonPairFeatures(instance, EMR, GroundTruth, classValue);
        }
    }
}
