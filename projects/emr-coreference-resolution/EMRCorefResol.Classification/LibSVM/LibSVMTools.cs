using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using static HCMUT.EMRCorefResol.Logging.LoggerFactory;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    static class SVMType
    {
        public const int C_SVC = 0;
        public const int NU_SVC = 1;
        public const int ONE_CLASS = 2;
        public const int EPSILON_SVR = 3;
        public const int NU_SVR = 4;
    }

    static class SVMKernel
    {
        public const int LINEAR = 0;
        public const int POLY = 1;
        public const int RBF = 2;
        public const int SIGMOID = 3;
    }

    static class LibSVMTools
    {
        public static string ToolsPath { get; } = "LibSVM";
        public static string SVMTrainPath { get; } = Path.Combine(ToolsPath, "svm-train.exe");
        public static string SVMScalePath { get; } = Path.Combine(ToolsPath, "svm-scale.exe");
        public static string SVMPredictPath { get; } = Path.Combine(ToolsPath, "svm-predict.exe");

        public static void RunSVMScale(double lower, double upper, string sfPath, string dataPath, string scaledDataPath)
        {
            var pInfo = new ProcessStartInfo()
            {
                FileName = SVMScalePath,
                Arguments = $"-l {lower} -u {upper} -s {sfPath} {dataPath}",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var p = Process.Start(pInfo);

            var sw = new StreamWriter(scaledDataPath);
            sw.Write(p.StandardOutput.ReadToEnd());
            sw.Close();
        }

        public static void RunSVMScale(string sfPath, string dataPath, string scaledDataPath)
        {
            var pInfo = new ProcessStartInfo()
            {
                FileName = SVMScalePath,
                Arguments = $"-r {sfPath} {dataPath}",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var p = Process.Start(pInfo);

            var sw = new StreamWriter(scaledDataPath);
            sw.Write(p.StandardOutput.ReadToEnd());
            sw.Close();
        }

        public static void RunSVMTrain(int type, int kernel, string dataPath, string modelPath)
        {
            var pInfo = new ProcessStartInfo()
            {
                FileName = SVMTrainPath,
                Arguments = $"-s {type} -t {kernel} {dataPath} {modelPath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            var p = Process.Start(pInfo);
            GetLogger().Info(p.StandardOutput.ReadToEnd());
        }

        public static void RunSVMPredict(string dataPath, string modelPath, string outputPath)
        {
            var pInfo = new ProcessStartInfo()
            {
                FileName = SVMPredictPath,
                Arguments = $"{dataPath} {modelPath} {outputPath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            var p = Process.Start(pInfo);
            GetLogger().Info(p.StandardOutput.ReadToEnd());
        }
    }
}
