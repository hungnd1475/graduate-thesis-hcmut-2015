using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.English.Features
{
    class CosineDistanceFeature : Feature
    {
        public CosineDistanceFeature(IConceptPair instance)
            : base("Cosine-Distance")
        {
            var anaUnique = Unique(instance.Anaphora.Lexicon);
            var anteUnique = Unique(instance.Antecedent.Lexicon);
            var unique = CombineUnique(anaUnique, anteUnique);

            var anaVector = Vectorize(unique, instance.Anaphora.Lexicon);
            var anteVector = Vectorize(unique, instance.Antecedent.Lexicon);

            var cosineValue = ComputeCosineDistance(anaVector, anteVector);

            SetContinuousValue(cosineValue);
        }

        private string[] Unique(string term)
        {
            var words = term.ToLower().Split(' ');
            return words.Distinct().ToArray();
        }

        private string[] CombineUnique(string[] arr1, string[] arr2)
        {
            return arr1.Union(arr2).ToArray();
        }

        private int[] Vectorize(string[] unique, string term)
        {
            List<int> res = new List<int>();
            var words = term.Split(' ');
            foreach(string s in unique)
            {
                int count = words.Count(f => f.Equals(s, StringComparison.InvariantCultureIgnoreCase));
                res.Add(count);
            }

            return res.ToArray();
        }

        private double ComputeCosineDistance(int[] vector1, int[] vector2)
        {
            var dotValue = ComputeDotValue(vector1, vector2);
            var vector1Length = ComputeLength(vector1);
            var vector2Length = ComputeLength(vector2);

            var cosineValue = Math.Abs(dotValue / (vector1Length * vector2Length));
            return cosineValue;
        }

        private int ComputeDotValue(int[] vector1, int[] vector2)
        {
            var dotValue = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                dotValue += vector1[i] * vector2[i];
            }

            return dotValue;
        }

        private double ComputeLength(int[] vector)
        {
            var length = 0;
            foreach(int i in vector)
            {
                length += i * i;
            }

            return Math.Sqrt(length);
        }
    }
}
