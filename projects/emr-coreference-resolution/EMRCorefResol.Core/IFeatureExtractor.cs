using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public interface IFeatureExtractor
	{
		IFeatureVector Extract(PersonPair instance, string emr);
		IFeatureVector Extract(ProblemPair instance, string emr);
		IFeatureVector Extract(TreatmentPair instance, string emr);
		IFeatureVector Extract(TestPair instance, string emr);
		IFeatureVector Extract(PronounInstance instance, string emr);
	}
}
