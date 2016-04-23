using HCMUT.EMRCorefResol;

namespace EMRCorefResol.TestingGUI
{
    class EMRChangedEventArgs
    {
        public EMR EMR { get; }
        public CorefChainCollection GroundTruth { get; }

        public EMRChangedEventArgs(EMR emr, CorefChainCollection groundTruth)
        {
            EMR = emr;
            GroundTruth = groundTruth;
        }
    }
}
