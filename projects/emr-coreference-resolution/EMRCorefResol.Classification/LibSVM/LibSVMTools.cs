using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    static class LibSVMTools
    {
        public static string ToolsPath { get; } = "LibSVM";
        public static string SVMTrainPath { get; } = Path.Combine(ToolsPath, "svm-train.exe");
        public static string SVMScalePath { get; } = Path.Combine(ToolsPath, "svm-scale.exe");
        public static string SVMPredictPath { get; } = Path.Combine(ToolsPath, "svm-predict.exe");
    }
}
