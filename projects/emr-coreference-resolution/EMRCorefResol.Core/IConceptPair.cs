﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface IConceptPair
    {
        Concept Antecedent { get; }
        Concept Anaphora { get; }
    }
}
