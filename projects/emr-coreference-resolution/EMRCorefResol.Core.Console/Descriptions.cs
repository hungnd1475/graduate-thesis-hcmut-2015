using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Core.Console
{
    public static class Descriptions
    {
        static readonly string[] ClasMethodDescs = new string[]
        {
            "    + LibSVM: 1"
        };

        static readonly string[] InstanceDescs = new string[]
        {
            "    + PersonInstance: 1",
            "    + PersonPair: 2",
            "    + PronounInstance: 3",
            "    + ProblemPair: 4",
            "    + TestPair: 5",
            "    + TreatmentPair: 6"
        };

        static readonly string[] EMRFormatDescs = new string[]
        {
            "    + I2B2: 1"
        };

        static readonly string[] LanguageDescs = new string[]
        {
            "    + English: 1",
            "    + Vietnamese: 2"
        };

        static readonly string[] ModeDescs = new string[]
        {
            "    + Train: 1",
            "    + Test: 2",
            "    + Classify: 3"
        };

        static readonly string[] ResolMethodDescs = new string[]
        {
            "    + Best first: 1"
        };

        public static string ClasMethod(string defVal)
        {
            return $"Set classification method. Can be one of the followings ({GetDefVal(defVal)}):\n" +
                $"{string.Join(Environment.NewLine, ClasMethodDescs)}";
        }

        public static string Instance(string defVal)
        {
            return $"Set instance type. Can be one of the followings ({GetDefVal(defVal)}):\n"
               + $"{string.Join(Environment.NewLine, InstanceDescs)}";
        }

        public static string EMRFormat(string defVal)
        {
            return $"Set EMR format. Can be one of the followings ({GetDefVal(defVal)}):\n"
                + $"{string.Join(Environment.NewLine, EMRFormatDescs)}";
        }

        public static string Language(string defVal)
        {
            return $"Set EMR language. Can be one of the followings ({GetDefVal(defVal)}):\n"
                + $"{string.Join(Environment.NewLine, LanguageDescs)}";
        }

        public static string Mode(string defVal)
        {
            return $"Set operation mode. Can be one of the followings ({GetDefVal(defVal)}):\n"
                + string.Join(Environment.NewLine, ModeDescs);
        }
        
        public static string ResolMethod(string defVal)
        {
            return $"Set resolution method. Can be one of the followings ({GetDefVal(defVal)}):\n"
                + string.Join(Environment.NewLine, ResolMethodDescs);
        }

        private static string GetDefVal(string defVal)
        {
            return !string.IsNullOrEmpty(defVal) ? "default " + defVal : "required";
        }
    }
}
