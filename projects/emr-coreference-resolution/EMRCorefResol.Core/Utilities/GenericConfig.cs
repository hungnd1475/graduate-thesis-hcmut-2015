using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    public class GenericConfig
    {
        private readonly Dictionary<string, object> _values
            = new Dictionary<string, object>();

        public void SetConfig(string name, object value)
        {
            _values[name] = value;
        } 

        public bool TryGetConfig<T>(string name, out T value)
        {
            try
            {
                value = (T)_values[name];
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }
    }
}
