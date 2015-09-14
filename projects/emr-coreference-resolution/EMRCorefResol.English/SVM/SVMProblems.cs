using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp;

namespace HCMUT.EMRCorefResol.English.SVM
{
    class SVMProblems
    {
        public SVMProblem PersonPair { get; } = new SVMProblem();
        public SVMProblem PersonInstance { get; } = new SVMProblem();
        public SVMProblem ProblemPair { get; } = new SVMProblem();
        public SVMProblem TreatmentPair { get; } = new SVMProblem();
        public SVMProblem TestPair { get; } = new SVMProblem();
        public SVMProblem PronounInstance { get; } = new SVMProblem();

        public void Add(PersonPairFeatures f)
        {
            PersonPair.Add(f.ToSVMNodes(), f.ClassValue);
        }
    }
}
