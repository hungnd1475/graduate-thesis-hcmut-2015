using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public abstract class SingleInstance : IClasInstance, ISingleConcept
    {
        public Concept Concept { get; }

        public SingleInstance(Concept concept)
        {
            this.Concept = concept;
        }

        public abstract IFeatureVector GetFeatures(IFeatureExtractor extractor);

        public override string ToString()
        {
            return Concept.ToString();
        }
    }

    public class PronounInstance : SingleInstance
    {
        public PronounInstance(Concept pronoun)
            : base(pronoun)
        { }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }

    public class PersonInstance : SingleInstance
    {
        public PersonInstance(Concept person)
            : base(person)
        { }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }
}
