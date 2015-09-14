using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface ITrainer
    {
        TrainingResult TrainFromFile(string emrFile, string conFile, string chainFile,
            IDataReader dataReader, IPreprocessor preprocessor);
        TrainingResult TrainFromDir(string emrDir, string conDir, string chainDir,
            IDataReader dataReader, IPreprocessor preprocessor);
    }
}
