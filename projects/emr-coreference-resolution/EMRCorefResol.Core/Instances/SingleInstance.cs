using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol
{
    public abstract class SingleInstance : IClasInstance, ISingleConcept, IEquatable<SingleInstance>
    {
        public Concept Concept { get; }

        public SingleInstance(Concept concept)
        {
            this.Concept = concept;
        }

        public abstract IFeatureVector GetFeatures(IFeatureExtractor extractor);

        public override string ToString()
        {
            return $"{Concept.ToString()}||t=\"{Concept.Type.ToString().ToLower()}";
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Concept);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SingleInstance);
        }

        public abstract void AddTo(ClasProblemCollection pCreator, IFeatureVector fVector);
        
        public abstract ClasResult Classify(IClassifier classifier, IFeatureVector fVector);

        public bool Equals(SingleInstance other)
        {
            return other == null ? false : Concept.Equals(other);
        }
    }

    public class PronounInstance : SingleInstance
    {
        public PronounInstance(Concept pronoun)
            : base(pronoun)
        { }

        public override void AddTo(ClasProblemCollection pCreator, IFeatureVector fVector)
        {
            pCreator.Add(this, fVector);
        }

        public override ClasResult Classify(IClassifier classifier, IFeatureVector fVector)
        {
            return classifier.Classify(this, fVector);
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

        public override void AddTo(ClasProblemCollection pCreator, IFeatureVector fVector)
        {
            pCreator.Add(this, fVector);
        }

        public override ClasResult Classify(IClassifier classifier, IFeatureVector fVector)
        {
            return classifier.Classify(this, fVector);
        }

        public override IFeatureVector GetFeatures(IFeatureExtractor extractor)
        {
            return extractor.Extract(this);
        }
    }
}
