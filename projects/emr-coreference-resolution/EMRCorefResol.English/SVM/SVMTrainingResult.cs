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
            _problems.Save(dir);
        }

        public Task SaveProblemsAsync(string dir)
        {
            return Task.Run(() => SaveProblems(dir));
        }
    }
}
