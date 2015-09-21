using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Logging
{
    public interface ILogger
    {
        void Info(string s);
        void Debug(string s);
        void Error(string s);
    }
}
