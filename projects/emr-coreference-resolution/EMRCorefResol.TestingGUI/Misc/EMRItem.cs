using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    public class EMRItem
    {
        public int Index { get; }
        public string Name { get; }

        public EMRItem(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}
