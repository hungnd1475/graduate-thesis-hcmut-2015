using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public interface ITrainer<T> where T : TrainingResult
    {
        T TrainFromFile(string emrFile, string conFile, string chainFile,
            IDataReader dataReader, IPreprocessor preprocessor);

        Task<T> TrainFromFileAsync(string emrFile, string conFile, string chainFile,
            IDataReader dataReader, IPreprocessor preprocessor);

        T TrainFromDir(string emrDir, string conDir, string chainDir,
            IDataReader dataReader, IPreprocessor preprocessor);

        Task<T> TrainFromDirAsync(string emrDir, string conDir, string chainDir,
            IDataReader dataReader, IPreprocessor preprocessor);
    }
}
