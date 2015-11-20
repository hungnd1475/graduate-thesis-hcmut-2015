using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    using Classification;

    /// <summary>
    /// Represents a classification instance.
    /// </summary>
    public interface IClasInstance
    {
        /// <summary>
        /// Extracts the feature vector from this instance.
        /// </summary>
        /// <param name="extractor">The extractor used to extract the features.</param>
        /// <returns></returns>
        IFeatureVector GetFeatures(IFeatureExtractor extractor);

        void AddTo(ClasProblemCollection pCreator, IFeatureVector fVector);

        ClasResult Classify(IClassifier classifier, IFeatureVector fVector);
    }
}
