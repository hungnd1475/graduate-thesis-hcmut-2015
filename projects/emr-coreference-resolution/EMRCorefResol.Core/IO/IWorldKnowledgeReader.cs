using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.IO
{
    public interface IWorldKnowledgeReader<TKey, TValue>
    {        
        bool Read(string line, out TKey key, out TValue value);
    }
}
