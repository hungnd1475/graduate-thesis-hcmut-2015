using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HCMUT.EMRCorefResol.Classification.LibSVM.Internal;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HCMUT.EMRCorefResol.Classification.LibSVM
{
    enum SVMType : int
    {
        C_SVC = 0,
        NU_SVC = 1,
        ONE_CLASS = 2,
        EPSILON_SVR = 3,
        NU_SVR = 4
    }

    enum SVMKernel : int
    {
        LINEAR = 0,
        POLY = 1,
        RBF = 2,
        SIGMOID = 3
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LibSVMPrintFunction([In, MarshalAs(UnmanagedType.LPStr)] string s);

    static class LibSVM
    {
        public const string ToolsPath = "LibSVM";
        public const string SVMTrainPath = ToolsPath + "\\svm-train.exe";
        public const string SVMScalePath = ToolsPath + "\\svm-scale.exe";
        public const string SVMPredictPath = ToolsPath + "\\svm-predict.exe";
        public const string DllPath = ToolsPath + "\\libsvm.dll";

        private static readonly Regex CVOutputPattern = new Regex("Cross Validation Accuracy = (?<a>[\\w\\W]+)%\n");

        private static ProcessStartInfo CreateProcess(string fileName, string arguments)
        {
            return new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
        }

        public static void RunSVMScale(double lower, double upper, string sfPath, string dataPath, string scaledDataPath)
        {
            var pInfo = CreateProcess(SVMScalePath, $"-l {lower} -u {upper} -s \"{sfPath}\" \"{dataPath}\"");
            var p = Process.Start(pInfo);
            using (var sw = new StreamWriter(scaledDataPath))
            {
                sw.Write(p.StandardOutput.ReadToEnd());
            }
        }

        public static string RunSVMScale(double lower, double upper, string sfPath, string dataPath)
        {
            var pInfo = CreateProcess(SVMScalePath, $"-l {lower} -u {upper} -s \"{sfPath}\" \"{dataPath}\"");
            var p = Process.Start(pInfo);
            var output = p.StandardOutput.ReadToEnd();
            return output;
        }

        public static void RunSVMScale(string sfPath, string dataPath, string scaledDataPath)
        {
            var pInfo = CreateProcess(SVMScalePath, $"-r \"{sfPath}\" \"{dataPath}\"");
            var p = Process.Start(pInfo);
            using (var sw = new StreamWriter(scaledDataPath))
            {
                sw.Write(p.StandardOutput.ReadToEnd());
            }
        }

        public static string RunSVMScale(string sfPath, string dataPath)
        {
            var pInfo = CreateProcess(SVMScalePath, $"-r \"{sfPath}\" \"{dataPath}\"");
            var p = Process.Start(pInfo);
            return p.StandardOutput.ReadToEnd();
        }

        public static void RunSVMTrain(SVMParameter param, string dataPath, string modelPath, bool shouldLog)
        {
            var pInfo = CreateProcess(SVMTrainPath, CreateSVMTrainArgs(param, dataPath, modelPath));
            var p = Process.Start(pInfo);

            if (shouldLog)
            {
                while (!p.StandardOutput.EndOfStream)
                {
                    var c = (char)p.StandardOutput.Read();
                    Console.Write(c);
                }
            }
        }

        private static string CreateSVMTrainArgs(SVMParameter param, string dataPath, string modelPath)
        {
            var h = param.Shrinking ? 1 : 0;
            var b = param.Probability ? 1 : 0;
            var w = string.Join(" ", Enumerable.Range(0, param.Weights.Length)
                .Select(i => $"-w{param.WeightLabels[i]} {param.Weights[i]}"));

            return $"-s {(int)param.Type} -t {(int)param.Kernel} -d {param.Degree} -g {param.Gamma} -r {param.Coef0}"
                + $" -c {param.C} -n {param.Nu} -p {param.P} -m {param.CacheSize} -e {param.Eps} -h {h}"
                + $" -b {b} {w} \"{dataPath}\" \"{modelPath}\"";
        }

        public static void RunSVMPredict(string dataPath, string modelPath, string outputPath)
        {
            var pInfo = CreateProcess(SVMPredictPath, $"-b 1 \"{dataPath}\" \"{modelPath}\" \"{outputPath}\"");
            var p = Process.Start(pInfo);
            var output = p.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }

        public static SVMModel LoadModel(string modelPath)
        {
            if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
            {
                return null;
            }

            IntPtr ptr_filename = Marshal.StringToHGlobalAnsi(modelPath);
            IntPtr ptr_model = NativeLibSVM.LoadModel(ptr_filename);

            Marshal.FreeHGlobal(ptr_filename);
            ptr_filename = IntPtr.Zero;

            if (ptr_model == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                SVMModel model = SVMModel.Convert(ptr_model);

                // There is a little memory leakage here !!!
                NativeLibSVM.FreeModelContent(ptr_model);
                ptr_model = IntPtr.Zero;

                return model;
            }
        }

        public static double Predict(SVMModel model, SVMNode[] x)
        {
            double confidence;
            return Predict(model, x, out confidence);
        }

        public static double Predict(SVMModel model, SVMNode[] x, out double confidence)
        {
            var ptr_model = SVMModel.Allocate(model);
            if (ptr_model == IntPtr.Zero)
                throw new ArgumentNullException("model");

            var nodes = x.Select(a => a.Clone()).ToList();
            nodes.Add(new SVMNode(-1, 0));
            var ptr_nodes = SVMNode.Allocate(nodes.ToArray());

            var ptr_probs = Marshal.AllocHGlobal(Marshal.SizeOf<double>() * model.ClassCount);
            var label = NativeLibSVM.PredictProbability(ptr_model, ptr_nodes, ptr_probs);
            var lIndex = Enumerable.Range(0, model.ClassCount).First(i => model.Labels[i] == label);

            var probs = new double[model.ClassCount];
            Marshal.Copy(ptr_probs, probs, 0, model.ClassCount);

            Marshal.FreeHGlobal(ptr_probs);
            SVMModel.Free(ptr_model);
            SVMNode.Free(ptr_nodes);

            confidence = probs[lIndex];
            return label;
        }

        public static SVMProblem ReadProblem(string content)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            SVMProblem problem = new SVMProblem();

            using (var sr = new StringReader(content))
            {
                while (true)
                {
                    string line = sr.ReadLine();

                    if (line == null)
                        break;

                    string[] list = line.Trim().Split(' ');

                    double y = Convert.ToDouble(list[0].Trim(), provider);

                    List<SVMNode> nodes = new List<SVMNode>();
                    for (int i = 1; i < list.Length; i++)
                    {
                        string[] temp = list[i].Split(':');
                        SVMNode node = new SVMNode();
                        node.Index = Convert.ToInt32(temp[0].Trim());
                        node.Value = Convert.ToDouble(temp[1].Trim(), provider);
                        nodes.Add(node);
                    }

                    problem.Add(nodes.ToArray(), y);
                }
            }

            return problem;
        }

        public static double[] CrossValidation(SVMParameter parameter, SVMProblem problem, int nFolds)
        {
            IntPtr ptr_target = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * problem.Length);
            IntPtr ptr_problem = SVMProblem.Allocate(problem);
            IntPtr ptr_parameter = SVMParameter.Allocate(parameter);

            NativeLibSVM.CrossValidation(ptr_problem, ptr_parameter, nFolds, ptr_target);

            var target = new double[problem.Length];
            Marshal.Copy(ptr_target, target, 0, target.Length);

            SVMProblem.Free(ptr_problem);
            SVMParameter.Free(ptr_parameter);
            Marshal.FreeHGlobal(ptr_target);
            ptr_target = IntPtr.Zero;

            return target;
        }

        public static void CalcWeights(SVMProblem problem, out int[] labels, out double[] weights)
        {
            var classCounts = new Dictionary<int, int>();
            foreach (var y in problem.Y)
            {
                var yInt = (int)y;
                if (!classCounts.ContainsKey(yInt))
                {
                    classCounts.Add(yInt, 0);
                }
                else
                {
                    classCounts[yInt] += 1;
                }
            }

            var maxCount = classCounts.Values.Aggregate(int.MinValue, (max, count) => max < count ? count : max);
            labels = classCounts.Keys.ToArray();
            weights = classCounts.Values.Select(count => (double)maxCount / count).ToArray();
        }

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_set_print_string_function")]
        public static extern void SetPrintStringFunction([MarshalAs(UnmanagedType.FunctionPtr)] LibSVMPrintFunction func);

        static class NativeLibSVM
        {
            /// <param name="modelPath">string</param>
            /// <returns></returns>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_load_model")]
            public static extern IntPtr LoadModel(IntPtr modelPath);

            /// <param name="model">svm_model</param>
            /// <param name="x">svm_node[]</param>
            /// <returns></returns>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_predict")]
            public static extern double Predict(IntPtr model, IntPtr x);

            /// <param name="model">svm_model</param>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_free_model_content")]
            public static extern void FreeModelContent(IntPtr model);

            /// <param name="prob">svm_problem</param>
            /// <param name="param">svm_parameter</param>
            /// <param name="nr_fold">int</param>
            /// <param name="target">double[]</param>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_cross_validation")]
            public static extern void CrossValidation(IntPtr prob, IntPtr param, int nr_fold, IntPtr target);

            /// <param name="model">svm_model</param>
            /// <param name="x">svm_node[]</param>
            /// <param name="values">double[]</param>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_predict_values")]
            public static extern double PredictValues(IntPtr model, IntPtr x, IntPtr values);

            /// <param name="model">svm_model</param>
            /// <param name="x">svm_node[]</param>
            /// <param name="probs">double[]</param>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_predict_probability")]
            public static extern double PredictProbability(IntPtr model, IntPtr x, IntPtr probs);

            /// <param name="model">svm_model</param>
            /// <returns></returns>
            [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "svm_check_probability_model")]
            public static extern bool CheckProbabilityModel(IntPtr model);
        }
    }
}
