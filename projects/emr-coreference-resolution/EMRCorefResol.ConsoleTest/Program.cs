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
    using System.Text.RegularExpressions;

    class Program
    {

        static void Main(string[] args)
        {
            ReadExtractedFile();
            Console.ReadLine();
        }

        static void ReadExtractedFile()
        {
            var collection = new UMLSDataDictionary(@"E:\graduate-thesis-hcmut-2015\dataset\i2b2_Train\umls\clinical-11.txt");
        }

        static void testClassifier()
        {
            var path = @"D:\Documents\Visual Studio 2015\Projects\graduate-thesis-hcmut-2015\dataset\i2b2_Beth_Train_Release.tar\i2b2_Beth_Train\Beth_Train";
            var emrName = "clinical-108.txt";
            var emrFile = Path.Combine(path, "docs", emrName);
            var conceptsFile = Path.Combine(path, "concepts", $"{emrName}.con");
            var chainsFile = Path.Combine(path, "chains", $"{emrName}.chains");

            var classifier = new LibSVMClassifier(@"D:\Documents\HCMUT\AI Research\Testing\19102015\Models");            
        }

        static void testCEAF(int[] c1, int[] c2)
        {
            if (c1.Length > c2.Length)
            {
                GenericHelper.Swap(ref c1, ref c2);
            }

            int[] bestMap;
            var bestPhi = findBestPhi(c1, c2, out bestMap);
            Console.WriteLine(string.Join(" ", Enumerable.Range(0, c1.Length).Select(i => $"{c1[i]}-{c2[bestMap[i]]}")) + $" -> {bestPhi:N3}");
        }

        static double phi4(double value1, double value2)
        {
            return 2 * value1 * value2 / (value1 + value2);
        }

        static double findBestPhi(int[] c1, int[] c2, out int[] bestMap)
        {
            var n = c2.Length;

            var phiMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i < c1.Length)
                    {
                        phiMatrix[i, j] = phi4(c1[i], c2[j]);
                    }
                    else
                    {
                        phiMatrix[i, j] = 0d;
                    }
                }
            }

            normalize(phiMatrix, n);

            // step 1
            reduceRows(phiMatrix, n);

            // step 2
            var starredZeros = new bool[n, n];
            starMatrix(phiMatrix, n, starredZeros);

            var coveredRows = new bool[n];
            var coveredCols = new bool[n];
            var primedZeros = new bool[n, n];

            while (true)
            {
                // step 3
                var nColsCovered = coverCols(coveredCols, starredZeros, n);

                if (nColsCovered == n)
                {
                    // goto step DONE.
                    break;
                }

                // step 4
                double min = double.MaxValue;
                Coordinate primeZ;

                while (!primeMatrix(phiMatrix, n, starredZeros, primedZeros, coveredRows, coveredCols,
                    out min, out primeZ))
                {
                    // step 6
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (coveredRows[i])
                            {
                                phiMatrix[i, j] += min;
                            }

                            if (!coveredCols[j])
                            {
                                phiMatrix[i, j] -= min;
                            }
                        }
                    }
                }

                // step 5
                while (true)
                {
                    var isDone = true;
                    starredZeros[primeZ.RowIndex, primeZ.ColIndex] = true;

                    for (int i = 0; i < n; i++)
                    {
                        if (i != primeZ.RowIndex)
                        {
                            var j = primeZ.ColIndex;
                            if (starredZeros[i, j])
                            {
                                starredZeros[i, j] = false;
                                isDone = false;

                                for (int tj = 0; tj < n; tj++)
                                {
                                    if (primedZeros[i, tj])
                                    {
                                        primeZ = new Coordinate(i, tj);
                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    if (isDone)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            coveredRows[i] = false;
                            coveredCols[i] = false;

                            for (int j = 0; j < n; j++)
                            {
                                primedZeros[i, j] = false;
                            }
                        }
                        break;
                    }
                }
            }

            // done
            var bestPhi = 0d;
            bestMap = new int[c1.Length];

            for (int i = 0; i < c1.Length; i++)
            {
                for (int j = 0; j < c2.Length; j++)
                {
                    if (starredZeros[i, j])
                    {
                        bestPhi += phi4(c1[i], c2[j]);
                        bestMap[i] = j;
                        break;
                    }
                }
            }

            return bestPhi;
        }

        static void starMatrix(double[,] matrix, int n, bool[,] starredCells)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] == 0d)
                    {
                        var starred = false;

                        for (int pi = 0; pi <= i; pi++)
                        {
                            for (int pj = 0; pj <= j; pj++)
                            {
                                if (starredCells[pi, pj])
                                {
                                    starred = true;
                                    break;
                                }
                            }
                            if (starred) break;
                        }

                        if (!starred)
                        {
                            starredCells[i, j] = true;
                        }
                    }
                }
            }
        }

        static int coverCols(bool[] coveredCols, bool[,] starredCells, int n)
        {
            var nColCovered = 0;

            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (starredCells[i, j])
                    {
                        coveredCols[j] = true;
                        nColCovered += 1;
                        break;
                    }
                }
            }

            return nColCovered;
        }

        static bool primeMatrix(double[,] matrix, int n, bool[,] starredCells, bool[,] primedCells,
            bool[] coveredRows, bool[] coveredCols, out double min, out Coordinate primeZ)
        {
            min = double.MaxValue;
            primeZ = new Coordinate();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] == 0d && !coveredCols[j] && !coveredRows[i])
                    {
                        primedCells[i, j] = true;
                        var gotoStep5 = true;
                        var starredCol = 0;

                        for (int tj = 0; tj < n; tj++)
                        {
                            if (starredCells[i, tj])
                            {
                                gotoStep5 = false;
                                starredCol = tj;
                                break;
                            }
                        }

                        if (gotoStep5)
                        {
                            primeZ = new Coordinate(i, j);
                            return true;
                        }
                        else
                        {
                            coveredRows[i] = true;
                            coveredCols[starredCol] = false;

                            if (!hasUncoveredZeros(matrix, n, coveredRows, coveredCols))
                            {
                                min = findMin(matrix, n, coveredRows, coveredCols);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        static double findMin(double[,] matrix, int n, bool[] coveredRows, bool[] coveredCols)
        {
            var min = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!coveredRows[i] && !coveredCols[j])
                    {
                        if (min > matrix[i, j])
                        {
                            min = matrix[i, j];
                        }
                    }
                }
            }

            return min;
        }

        static bool hasUncoveredZeros(double[,] matrix, int n, bool[] coveredRows, bool[] coveredCols)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] == 0d && !coveredRows[i] && !coveredCols[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static void normalize(double[,] matrix, int n)
        {
            var max = double.MinValue;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (max < matrix[i, j])
                    {
                        max = matrix[i, j];
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = max - matrix[i, j];
                }
            }
        }

        static void reduceRows(double[,] matrix, int n)
        {
            for (int i = 0; i < n; i++)
            {
                var min = double.MaxValue;
                for (int j = 0; j < n; j++)
                {
                    var c = matrix[i, j];
                    if (min > c)
                    {
                        min = c;
                    }
                }

                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] -= min;
                }
            }
        }

        struct Coordinate
        {
            public int RowIndex { get; }
            public int ColIndex { get; }

            public Coordinate(int rowIndex, int colIndex)
                : this()
            {
                RowIndex = rowIndex;
                ColIndex = colIndex;
            }
        }
    }
}
