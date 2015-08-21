﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IFeatureVector : IIndexedEnumerable<IFeature>
    {
        IFeature this[string name] { get; }
        double[] ToDoubleArray();
    }
}
