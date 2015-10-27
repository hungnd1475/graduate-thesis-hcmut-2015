using HCMUT.EMRCorefResol.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol.Evaluations
{
    public class CEAFPerfMetric : IPerfMetric
    {
        public string Name
        {
            get { return "CEAF"; }
        }

        public Dictionary<ConceptType, Evaluation> Evaluate(EMR emr, CorefChainCollection groundTruth, CorefChainCollection systemChains)
        {
            var evals = new Dictionary<ConceptType, Evaluation>();

            foreach (var type in Evaluation.ConceptTypes)
            {
                var tGroundTruth = groundTruth.GetChainsOfType(type);
                var tSystemChains = systemChains.GetChainsOfType(type);

                var c1 = tGroundTruth;
                var c2 = tSystemChains;
                if (c1.Count > c2.Count)
                {
                    GenericHelper.Swap(ref c1, ref c2);
                }

                double bestPhi = 0d;
                bestPhi = findBestPhi(c1, c2);

                var bothNone = c1.Count == 0 && c2.Count == 0;
                var p = bothNone ? 1d : bestPhi / tSystemChains.Aggregate(0d, (t, s) => t + phi4(s, s));
                var r = bothNone ? 1d : bestPhi / tGroundTruth.Aggregate(0d, (t, g) => t + phi4(g, g));
                var f = 2 * p * r / (p + r);

                evals.Add(type, new Evaluation(p, r, f, Name));
            }

            return evals;
        }

        private static double phi4(CorefChain a, CorefChain b)
        {
            var o = a.Intersect(b);
            return 2d * o.Count / (a.Count + b.Count);
        }

        private static double findBestPhi(CorefChainCollection c1, CorefChainCollection c2)
        {
            var n = c2.Count;
            var phiMatrix = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i < c1.Count)
                    {
                        phiMatrix[i, j] = phi4(c1[i], c2[j]);
                    }
                    else
                    {
                        phiMatrix[i, j] = 0d;
                    }
                }
            }

            // convert matrix to minimization problem
            toMinimization(phiMatrix, n);

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

            for (int i = 0; i < c1.Count; i++)
            {
                for (int j = 0; j < c2.Count; j++)
                {
                    if (starredZeros[i, j])
                    {
                        bestPhi += phi4(c1[i], c2[j]);
                        break;
                    }
                }
            }

            return bestPhi;
        }

        private static void starMatrix(double[,] matrix, int n, bool[,] starredCells)
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

        private static int coverCols(bool[] coveredCols, bool[,] starredCells, int n)
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

        private static bool primeMatrix(double[,] matrix, int n, bool[,] starredCells, bool[,] primedCells,
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

        private static double findMin(double[,] matrix, int n, bool[] coveredRows, bool[] coveredCols)
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

        private static bool hasUncoveredZeros(double[,] matrix, int n, bool[] coveredRows, bool[] coveredCols)
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

        private static void toMinimization(double[,] matrix, int n)
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

        private static void reduceRows(double[,] matrix, int n)
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
