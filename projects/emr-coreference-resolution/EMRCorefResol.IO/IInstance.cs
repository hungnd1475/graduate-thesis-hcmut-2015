using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	interface IInstance
	{
		Concept Antecedent { get; }
		Concept Anaphora { get; }
		double[] GetFeatures(IFeatureExtractor extractor, string summaryContent);
	}
}
