using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSVMsharp.Helpers;
using System.IO;

namespace HCMUT.EMRCorefResol.English.SVM
{
    public class SVMTrainingResult : TrainingResult
    {
        private readonly SVMProblems _problems;

        internal SVMTrainingResult(long completionTime, IClassifier classifier, SVMProblems problems)
            : base(completionTime, classifier)
        {
            _problems = problems;
        }

        public void SaveProblems(string dir)
        {
            SVMProblemHelper.Save(_problems.PersonPair, Path.Combine(dir, "personpair.prb"));
            // TODO save other svm problems here
        }

        public Task SaveProblemsAsync(string dir)
        {
            return Task.Run(() => SaveProblems(dir));
        }
    }
}
