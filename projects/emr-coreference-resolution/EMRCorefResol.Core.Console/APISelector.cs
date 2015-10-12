using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.IO;
using HCMUT.EMRCorefResol.English;
using HCMUT.EMRCorefResol.Classification.LibSVM;

namespace HCMUT.EMRCorefResol.Core.Console
{
    public static class APISelector
    {
        public static IDataReader SelectDataReader(EMRFormat emrFormat)
        {
            switch (emrFormat)
            {
                case EMRFormat.I2B2:
                    return new I2B2DataReader();
                default:
                    throw new ArgumentException("Cannot create an instance of IDataReader based on dataFormat value.");
            }
        }

        public static IFeatureExtractor SelectFeatureExtractor(Language language, Mode mode, ClasMethod method, string modelsDir)
        {
            if (mode == Mode.Train)
            {
                switch (language)
                {
                    case Language.English:
                        return new EnglishTrainingFeatureExtractor();
                    case Language.Vietnamese:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                switch (language)
                {
                    case Language.English:
                        return new EnglishClasFeatureExtractor(SelectClassifier(method, modelsDir));
                    case Language.Vietnamese:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public static IClasProblemSerializer SelectProblemSerializer(ClasMethod fformat)
        {
            switch (fformat)
            {
                case ClasMethod.LibSVM:
                    return LibSVMProblemSerializer.Instance;
                default:
                    throw new ArgumentException();
            }
        }

        public static IClassifier SelectClassifier(ClasMethod method, string modelsDir)
        {
            switch (method)
            {
                case ClasMethod.LibSVM:
                    return new LibSVMClassifier(modelsDir);
                default:
                    throw new ArgumentException();
            }
        }

        public static Type SelectInstanceType(Instance instanceType)
        {
            switch (instanceType)
            {
                case Instance.PersonInstance:
                    return typeof(PersonInstance);
                case Instance.PersonPair:
                    return typeof(PersonPair);
                case Instance.ProblemPair:
                    return typeof(ProblemPair);
                case Instance.PronounInstance:
                    return typeof(PronounInstance);
                case Instance.TestPair:
                    return typeof(TestPair);
                case Instance.TreatmentPair:
                    return typeof(TreatmentPair);
                default:
                    throw new ArgumentException("instanceType");
            }
        }

        public static ITrainer SelectTrainer(ClasMethod method, string outDir)
        {
            switch (method)
            {
                case ClasMethod.LibSVM:
                    return new LibSVMTrainer(outDir);
                default:
                    throw new ArgumentException("method");
            }
        }
    }
}
