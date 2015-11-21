using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCMUT.EMRCorefResol.IO;
using System.IO;

namespace HCMUT.EMRCorefResol
{
    public abstract class WorldKnowledgeDictionary<TKey, TValue>
    {
        public WorldKnowledgeDictionary(string fileName, IWorldKnowledgeReader<TKey, TValue> reader)
        {
            using (var sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    TKey key; TValue value;
                    if (reader.Read(line, out key, out value))
                    {
                        Add(key, value);
                    }
                }
            }
        }

        protected abstract void Add(TKey key, TValue value);
        public abstract TValue Get(TKey key);
    }
}
