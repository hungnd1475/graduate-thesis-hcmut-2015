﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface ITrainingFeatureExtractor : IFeatureExtractor
    {
        CorefChainCollection GroundTruth { get; set; }
    }
}
