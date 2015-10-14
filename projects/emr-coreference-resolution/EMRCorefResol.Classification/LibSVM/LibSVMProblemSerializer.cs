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
            Serialize(problem, filePath, false);
        }

        public void Serialize(ClasProblem problem, string filePath, bool doScale)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var dirName = Path.GetDirectoryName(filePath);

            if (doScale)
            {
                var tmpPath = Path.Combine(dirName, fileName + ".prb.tmp");
                SerializeInternal(problem, tmpPath);

                var sfPath = Path.Combine(dirName, fileName + ".sf");
                LibSVM.RunSVMScale(0d, 1d, sfPath, tmpPath, filePath);
                File.Delete(tmpPath);
            }
            else
            {
                SerializeInternal(problem, filePath);
            }
        }

        private void SerializeInternal(ClasProblem problem, string filePath)
        {
            using (var sw = new StreamWriter(filePath))
            {
                for (int i = 0; i < problem.Size; i++)
                {
                    sw.Write($"{problem.Y[i]} ");
                    int j = 1;

                    var X = problem.X[i];
                    for (int x = 0; x < X.Length; x++)
                    {
                        for (int xx = 0; xx < X[x].Length; xx++)
                        {
                            sw.Write($"{j++}:{X[x][xx]} ");
                        }
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}
