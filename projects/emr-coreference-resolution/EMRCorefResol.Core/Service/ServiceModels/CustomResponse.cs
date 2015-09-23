using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class CustomResponse
    {
        public bool success { get; set; }
        public Object data { get; set; }
        public string message { get; set; }
    }
}
