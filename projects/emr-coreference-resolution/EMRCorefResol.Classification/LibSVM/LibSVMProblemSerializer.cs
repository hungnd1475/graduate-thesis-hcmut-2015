using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    public class LibSVMProblemSerializer : IClasProblemSerializer
    {
        public static LibSVMProblemSerializer Instance { get; } = new LibSVMProblemSerializer();

        private LibSVMProblemSerializer() { }

        public ClasProblem Deserialize(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ClasProblem problem, string filePath)
        {
            var sw = new StreamWriter(filePath);
            for (int i = 0; i < problem.Size; i++)
            {
                sw.WriteLine($"{problem.Y[i]} {string.Join(" ", Enumerable.Range(0, problem.X[i].Size).Select(j => $"{j + 1}:{problem.X[i][j].Value}"))}");
            }
            sw.Close();
        }
    }
}
