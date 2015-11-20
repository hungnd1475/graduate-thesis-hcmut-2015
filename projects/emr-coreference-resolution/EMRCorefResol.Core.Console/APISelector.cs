using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.IO;
using HCMUT.EMRCorefResol.English;
using HCMUT.EMRCorefResol.Classification.LibSVM;
using HCMUT.EMRCorefResol.CorefResolvers;

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

        public static IFeatureExtractor SelectFeatureExtractor(Language language, IClassifier classifier)
        {
            switch (language)
            {
                case Language.English:
                    return new EnglishClasFeatureExtractor(classifier);
                case Language.Vietnamese:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException();
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

        public static ICorefResolver SelectResolver(ResolMethod method)
        {
            switch (method)
            {
                case ResolMethod.BestFirst:
                    return new BestFirstResolver();
                default:
                    throw new ArgumentException("method");
            }
        }

        public static IInstancesGenerator SelectInstancesGenerator(InstancesGenerator instGen, FilterRule filterRule)
        {
            switch (instGen)
            {
                case InstancesGenerator.All:
                    return new AllInstancesGenerator();
                case InstancesGenerator.ModNg2002:
                    return new ModNg2002InstancesGenerator(SelectFilterRule(filterRule));
                case InstancesGenerator.ModSoon2001:
                    return new ModSoon2001ModInstancesGenerator();
                case InstancesGenerator.Soon2001:
                    return new Soon2001InstancesGenerator();
                default:
                    throw new ArgumentException("instGen");
            }
        }

        public static IFilterRule SelectFilterRule(FilterRule filterRule)
        {
            switch (filterRule)
            {
                case FilterRule.True:
                    return new TrueFilterRule();
                case FilterRule.False:
                    return new FalseFilterRule();
                case FilterRule.SubstringMatch:
                    return new SubstringMatchFilterRule();
                default:
                    throw new ArgumentException("filterRule");
            }
        }
    }
}
