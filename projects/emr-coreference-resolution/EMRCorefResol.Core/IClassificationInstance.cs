using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public interface IClassificationInstance
	{
		double[] GetFeatures(IFeatureExtractor extractor, string emr);
	}
}
