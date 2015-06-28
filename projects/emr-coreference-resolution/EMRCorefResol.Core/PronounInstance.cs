using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public abstract class PronounInstance : IClassificationInstance
	{
		public Concept Pronoun { get; private set; }

		public PronounInstance(Concept pronoun)
		{
			this.Pronoun = pronoun;
		}

		public IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr)
		{
			return extractor.Extract(this, emr);
		}
	}
}
