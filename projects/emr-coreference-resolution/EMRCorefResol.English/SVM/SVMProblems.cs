using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp;
using LibSVMsharp.Helpers;
using System.IO;

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

        public void Add(PronounInstanceFeatures f)
        {
            PronounInstance.Add(f.ToSVMNodes(), f.ClassValue);
        }

        public void Add(PersonInstanceFeatures p)
        {
            PersonInstance.Add(p.ToSVMNodes(), p.ClassValue);
        }

        public void Save(string dirPath)
        {
            SVMProblemHelper.Save(PersonPair, Path.Combine(dirPath, "person-pair.prb"));
        }
    }
}
