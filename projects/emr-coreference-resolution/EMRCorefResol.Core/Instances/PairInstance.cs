using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Utilities;

namespace HCMUT.EMRCorefResol
{
    public abstract class PairInstance : IConceptPair, IClasInstance, IEquatable<PairInstance>
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

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Antecedent, Anaphora);
        }

        public abstract void AddTo(ClasProblemCollection pCreator, IFeatureVector fVector);

        public abstract ClasResult Classify(IClassifier classifier, IFeatureVector fVector);

        public bool Equals(PairInstance other)
        {
            return other == null ? false : 
                Anaphora.Equals(other.Anaphora) && Antecedent.Equals(other.Antecedent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PairInstance);
        }

        public static IClasInstance Create(Concept ante, Concept ana)
        {
            if (ante.Type != ana.Type)
            {
                throw new ArgumentException("Types of the two concepts does not match.");
            }

            switch (ante.Type)
            {
                case ConceptType.Person:
                    return new PersonPair(ante, ana);
                case ConceptType.Problem:
                    return new ProblemPair(ante, ana);
                case ConceptType.Test:
                    return new TestPair(ante, ana);
                case ConceptType.Treatment:
                    return new TreatmentPair(ante, ana);
                default:
                    throw new ArgumentException($"Cannot create pair instance with type {ante.Type}");
            }
        }
    }

    public class PersonPair : PairInstance 
    {
        public PersonPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

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

    public class ProblemPair : PairInstance 
    {
        public ProblemPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

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

    public class TreatmentPair : PairInstance 
    {
        public TreatmentPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

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

    public class TestPair : PairInstance 
    {
        public TestPair(Concept antecedent, Concept anaphora) : base(antecedent, anaphora) { }

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
