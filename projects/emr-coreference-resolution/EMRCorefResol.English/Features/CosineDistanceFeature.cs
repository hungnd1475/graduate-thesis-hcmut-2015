using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class CosineDistanceFeature : Feature
    {
        public CosineDistanceFeature(IConceptPair instance)
            : base("Cosine-Distance")
        {

        }
    }
}
