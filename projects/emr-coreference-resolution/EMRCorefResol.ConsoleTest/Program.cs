using HCMUT.EMRCorefResol.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using HCMUT.EMRCorefResol.Classification;
using HCMUT.EMRCorefResol.Classification.LibSVM;
using HCMUT.EMRCorefResol.Utilities;
using System.Threading;
using Fclp;

namespace HCMUT.EMRCorefResol.ConsoleTest
{
    using English;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var c1 = new int[] { 3, 6, 1, 4, 2, 5 };
            var c2 = new int[] { 2, 10, 9, 7 };

            var bestPhi = double.MinValue;
            var bestMap = new MapItem[c2.Length];
            var count = 0;

            findBestMap(c2, c1, 0, new MapItem[c2.Length], new HashSet<int>(), ref bestPhi, ref bestMap, ref count);

            Console.WriteLine($"Best phi: {bestPhi}");
            Console.WriteLine($"Best map: {string.Join(" ", bestMap.Select(m => $"{m.Value1}-{m.Value2}"))}");
            Console.WriteLine($"Count: {count}");
            Console.ReadLine();
        }

        static void findBestMap(int[] c1, int[] c2, int i, MapItem[] map, HashSet<int> mapped, ref double bestPhi,
            ref MapItem[] bestMap, ref int count)
        {
            for (int j = 0; j < c2.Length; j++)
            {                
                if (!mapped.Contains(j))
                {
                    map[i] = new MapItem(c1[i], c2[j]);
                    mapped.Add(j);

                    if (i < c1.Length - 1)
                    {
                        findBestMap(c1, c2, i + 1, map, mapped, ref bestPhi, ref bestMap, ref count);
                    }
                    else
                    {
                        var phi = map.Aggregate(0d, (p, m) => p + phi4(m.Value1, m.Value2));
                        count += 1;

                        Console.WriteLine($"Phi: {phi}");
                        Console.WriteLine($"Map: {string.Join(" ", map.Select(m => $"{m.Value1}-{m.Value2}"))}");
                        Console.WriteLine();                       
                        
                        if (phi > bestPhi)
                        {
                            map.CopyTo(bestMap, 0);
                            bestPhi = phi;
                        }
                    }

                    mapped.Remove(j);
                }
            }
        }

        static double phi4(double value1, double value2)
        {
            return 2 * value1 * value2 / (value1 + value2);
        }

        struct MapItem
        {
            public int Value1 { get; }
            public int Value2 { get; }

            public MapItem(int value1, int value2)
            {
                Value1 = value1;
                Value2 = value2;
            }
        }
    }
}
