using LibSVMsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.SVM
{
    static class ExtensionMethods
    {
        public static SVMNode[] ToSVMNodes(this IFeatureVector f)
        {
            var nodes = new SVMNode[f.Size];
            for (int i = 0; i < f.Size; i++)
                nodes[i] = new SVMNode(i, f[i].Value);
            return nodes;
        }
    }
}
