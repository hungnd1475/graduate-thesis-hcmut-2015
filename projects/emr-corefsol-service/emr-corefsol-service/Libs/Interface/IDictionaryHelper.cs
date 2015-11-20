using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emr_corefsol_service.Libs
{
    interface IDictionaryHelper
    {
        Models.Definition[] getSynSets(string term);
    }
}
