using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;

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

        public abstract void AddTo(ClasProblemCreator pCreator, IFeatureVector fVector);
    }

    public class PronounInstance : SingleInstance
    {
        public PronounInstance(Concept pronoun)
            : base(pronoun)
        { }

        public override void AddTo(ClasProblemCreator pCreator, IFeatureVector fVector)
        {
            pCreator.Add(this, fVector);
        }

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

        public override void AddTo(ClasProblemCreator pCreator, IFeatureVector fVector)
        {
            pCreator.Add(this, fVector);
        }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }
}
