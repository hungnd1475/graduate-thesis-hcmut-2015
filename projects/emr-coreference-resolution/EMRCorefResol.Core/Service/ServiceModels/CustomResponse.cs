using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Service
{
    public class CustomResponse
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
