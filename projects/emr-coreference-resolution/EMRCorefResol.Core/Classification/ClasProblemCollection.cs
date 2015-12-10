using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public class ClasProblemCollection
    {
        private readonly Dictionary<Type, ClasProblem> _problems 
            = new Dictionary<Type, ClasProblem>();

        public ClasProblemCollection()
        {
            _problems.Add(typeof(PersonInstance), new ClasProblem());
            _problems.Add(typeof(PersonPair), new ClasProblem());
            _problems.Add(typeof(ProblemPair), new ClasProblem());
            _problems.Add(typeof(PronounInstance), new ClasProblem());
            _problems.Add(typeof(TreatmentPair), new ClasProblem());
            _problems.Add(typeof(TestPair), new ClasProblem());
        }

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
            var p = _problems[instanceType];
            p.ClearCache();
            return p;
        }
    }
}
