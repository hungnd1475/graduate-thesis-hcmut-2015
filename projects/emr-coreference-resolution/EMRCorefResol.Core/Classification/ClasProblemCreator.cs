using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public class ClasProblemCreator
    {
        private readonly Dictionary<Type, ClasProblem> _problems 
            = new Dictionary<Type, ClasProblem>();

        public ClasProblemCreator()
        {
            _problems.Add(typeof(PersonInstance), new ClasProblem());
            _problems.Add(typeof(PersonPair), new ClasProblem());
            _problems.Add(typeof(ProblemPair), new ClasProblem());
            _problems.Add(typeof(PronounInstance), new ClasProblem());
            _problems.Add(typeof(TreatmentPair), new ClasProblem());
            _problems.Add(typeof(TestPair), new ClasProblem());
        }

        //public void Add(PersonInstance instance, IFeatureVector fVector)
        //{
        //    _problems[typeof(PersonInstance)].Add(fVector, fVector.ClassValue);
        //}

        //public void Add(PersonPair instance, IFeatureVector fVector)
        //{
        //    _problems[typeof(PersonPair)].Add(fVector, fVector.ClassValue);
        //}

        //public void Add(ProblemPair instance, IFeatureVector fVector)
        //{
        //    _problems[typeof(ProblemPair)].Add(fVector, fVector.ClassValue);
        //}

        //public void Add(PronounInstance instance, IFeatureVector fVector)
        //{
        //    _problems[typeof(PronounInstance)].Add(fVector, fVector.ClassValue);
        //}

        //public void Add(TreatmentPair instance, IFeatureVector fVector)
        //{
        //    _problems[typeof(TreatmentPair)].Add(fVector, fVector.ClassValue);
        //}

        //public void Add(TestPair instance, IFeatureVector fVector)
        //{
        //    _problems[typeof(TestPair)].Add(fVector, fVector.ClassValue);
        //}

        public void Add(IClasInstance instance, IFeatureVector fVector)
        {
            _problems[instance.GetType()].Add(fVector, fVector.ClassValue);
        }

        public ClasProblem GetProblem<TInstance>() where TInstance : IClasInstance
        {
            return GetProblem(typeof(TInstance));
        }

        public ClasProblem GetProblem(Type instanceType)
        {
            return _problems[instanceType];
        }
    }
}
