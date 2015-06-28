using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public abstract class PairInstance : IClassificationInstance
	{
		public Concept Antecedent { get; private set; }
		public Concept Anaphora { get; private set; }

		public PairInstance(Concept antecedent, Concept anaphora)
		{
			this.Antecedent = antecedent;
			this.Anaphora = anaphora;
		}

		public abstract IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr);
	}

	public class PersonPair : PairInstance 
	{
		public PersonPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

		public override IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr)
		{
			return extractor.Extract(this, emr);
		}
	}

	public class ProblemPair : PairInstance 
	{
		public ProblemPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

		public override IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr)
		{
			return extractor.Extract(this, emr);
		}
	}

	public class TreatmentPair : PairInstance 
	{
		public TreatmentPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

		public override IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr)
		{
			return extractor.Extract(this, emr);
		}
	}

	public class TestPair : PairInstance 
	{
		public TestPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

		public override IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr)
		{
			return extractor.Extract(this, emr);
		}
	}
}
