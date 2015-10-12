using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Classification
{
    public class ClasConfig
    {
        private readonly Dictionary<Type, GenericConfig> _configs
            = new Dictionary<Type, GenericConfig>();

        public void SetConfig<T>(GenericConfig config) where T : IClasInstance
        {
            SetConfig(typeof(T), config);
        }

        public void SetConfig(Type instanceType, GenericConfig config)
        {
            _configs[instanceType] = config;
        }

        public GenericConfig GetConfig<T>() where T : IClasInstance
        {
            return GetConfig(typeof(T));
        }

        public GenericConfig GetConfig(Type instanceType)
        {
            try { return _configs[instanceType]; }
            catch { return null; }
        }
    }
}
