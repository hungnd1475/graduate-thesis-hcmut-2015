using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    class LibSVMProblemSerializer : IClasProblemSerializer
    {
        public static LibSVMProblemSerializer Instance { get; } = new LibSVMProblemSerializer();

        private LibSVMProblemSerializer() { }

        public ClasProblem Load(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Save(ClasProblem problem, string filePath)
        {
            var sw = new StreamWriter(filePath);
            sw.Write(string.Join(Environment.NewLine,
                Enumerable.Range(0, problem.Size).Select(i =>
                    $"{problem.Y[i]} {string.Join(" ", Enumerable.Range(0, problem.X[i].Size).Select(j => $"{j + 1}:{problem.X[i][j].Value}"))}"
                ))
            );
        }
    }
}
