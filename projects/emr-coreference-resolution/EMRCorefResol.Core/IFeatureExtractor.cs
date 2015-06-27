using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public interface IFeatureExtractor
	{
		double[] Extract(PersonPair instance, string emr);
		double[] Extract(ProblemPair instance, string emr);
		double[] Extract(TreatmentPair instance, string emr);
		double[] Extract(TestPair instance, string emr);
		double[] Extract(PronounInstance instance, string emr);
	}
}
