using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Logging
{
    public interface ILogger
    {
        void WriteInfo(string s);
        void WriteDebug(string s);
        void WriteError(string s);

        void WriteInfo(string s, bool newLine);
        void UpdateInfo(string s);
    }
}
