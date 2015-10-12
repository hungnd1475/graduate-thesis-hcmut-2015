using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HCMUT.EMRCorefResol.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly object _lockObj = new object();

        public FileLogger(string filePath)
        {
            _filePath = filePath;
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        public void Log(string s)
        {
            s += Environment.NewLine;
            lock (_lockObj)
            {
                File.AppendAllText(_filePath, s);
            }
        }
    }
}
