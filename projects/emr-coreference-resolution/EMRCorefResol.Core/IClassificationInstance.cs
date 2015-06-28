using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public interface IClassificationInstance
	{
		IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr);
	}
}
