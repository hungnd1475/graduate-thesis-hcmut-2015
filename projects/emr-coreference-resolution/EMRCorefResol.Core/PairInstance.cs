using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public abstract class PairInstance : IConceptPair, IClasInstance
    {
        public Concept Antecedent { get; }
        public Concept Anaphora { get; }

        public PairInstance(Concept antecedent, Concept anaphora)
        {
            Antecedent = antecedent;
            Anaphora = anaphora;
        }

        public abstract IFeatureVector GetFeatures(IFeatureExtractor extractor);

        public override string ToString()
        {
            return $"{Antecedent}||{Anaphora}||t=\"{Antecedent.Type.ToString().ToLower()}\"";
        }
    }

    public class PersonPair : PairInstance 
    {
        public PersonPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }

    public class ProblemPair : PairInstance 
    {
        public ProblemPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }

    public class TreatmentPair : PairInstance 
    {
        public TreatmentPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }

    public class TestPair : PairInstance 
    {
        public TestPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }
}
