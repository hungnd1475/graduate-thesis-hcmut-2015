using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emr_corefsol_service.Libs
{
    interface INLPHelper
    {
        string[] getPOS(string term);
        string[] tokenize(string term);
    }
}
