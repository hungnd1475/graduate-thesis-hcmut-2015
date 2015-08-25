using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public abstract class PairInstance : IConceptPair, IClasInstance
    {
        public Concept Antecedent { get; set; }
        public Concept Anaphora { get; set; }

        public PairInstance(Concept antecedent, Concept anaphora)
        {
            this.Antecedent = antecedent;
            this.Anaphora = anaphora;
        }

        public abstract IFeatureVector GetFeatures(IFeatureExtractor extractor, string emr);

        public override string ToString()
        {
            return $"{Antecedent}||{Anaphora}||t=\"{Antecedent.Type.ToString().ToLower()}\"";
        }
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
