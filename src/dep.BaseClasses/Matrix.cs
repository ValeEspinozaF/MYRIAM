using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public partial class Matrix
    {
        /// <summary>
        /// Creates an empty matrix instance of the given rows and cols.
        /// </summary>
        /// <param name="rows">Amount of rows for the matrix.</param>
        /// <param name="cols">Amount of columns for the matrix.</param>
        /// <returns>Empty matrix of rows x cols dimensions.</returns>
        static double[,] Create(int rows, int cols)
        {
            double[,] result = new double[rows, cols];

            return result;
        }

        static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];

            return result;
        }

        static double[][] MatrixRebuild(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[][] result = new double[n][];
            for (int i = 0; i < n; ++i)
            {
                result[i] = new double[n];
                for (int j = 0; j < n; ++j)
                {
                    result[i][j] = matrix[i, j];
                }
            }

            return result;
        }

        static double[,] MatrixRebuild(double[][] matrix)
        {
            int n = matrix.Length;
            double[,] result = new double[n, n];
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i, j] = matrix[i][j];

            return result;
        }


        /// <summary>
        /// Checks whether the given matrix is an identity matrix.
        /// </summary>
        /// <param name="matrix">The matrix to evaluate.</param>
        /// <returns>True if matrix is an identity matrix, false otherwise.</returns>
        public static bool IsIdentity(double[,] matrix)
        {
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                if (matrix[i, i] != 1.0)
                    return false;

                for (int j = 0; j < n; j++)
                    if (i != j && matrix[i, j] != 0.0)
                        return false;
            }
            return true;
        }


        /// <summary>
        /// Checks whether the given matrix is filled with NaN values.
        /// </summary>
        /// <param name="matrix">The matrix to evaluate.</param>
        /// <returns>True if matrix is filled with NaN values, false otherwise.</returns>
        public static bool IsNaN(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (!double.IsNaN(matrix[i, j]))
                        return false;
                }

            return true;
        }

        /// <summary>
        /// Generates an identity matrix of side n.
        /// </summary>
        /// <param name="n">The dimensions or the output matrix.</param>
        /// <returns>A n x n identity matrix.</returns>
        public static double[,] CreateIdentity(int n)
        {
            double[,] result = Create(n, n);

            for (int i = 0; i < n; ++i)
                result[i, i] = 1.0;

            return result;
        }


        /// <summary>
        /// Calculates the dot-product between two matrices.
        /// </summary>
        /// <param name="matrixA">First matrix.</param>
        /// <param name="matrixB">Second matrix.</param>
        /// <returns>The result from matrixA * matrix B.</returns>
        public static double[,] DotProduct(double[,] matrixA, double[,] matrixB)
        {
            int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
            int bRows = matrixB.GetLength(0); int bCols = matrixB.GetLength(1);
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");

            double[,] result = Create(aRows, bCols);

            for (int i = 0; i < aRows; ++i)
                for (int j = 0; j < bCols; ++j)
                    for (int k = 0; k < aCols; ++k)
                        result[i, j] += matrixA[i, k] * matrixB[k, j];

            return result;
        }


        /// <summary>
        /// Calculates the transpose of the given matrix.
        /// </summary>
        /// <param name="matrix">The matrix to transpose.</param>
        /// <returns>The transpose.</returns>
        public static T[,] Transpose<T>(T[,] matrix)
        {
            var result = new T[matrix.GetLength(1), matrix.GetLength(0)];

            for (int j = 0; j < matrix.GetLength(1); j++)
                for (int i = 0; i < matrix.GetLength(0); i++)
                    result[j, i] = matrix[i, j];

            return result;
        }


        /// <summary>
        /// Calculates the inverse of the given square matrix. (Solution for C# by Dr. James McCaffrey)
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverse of a square matrix.</returns>
        public static double[,] Inverse(double[,] matrix)
        {
            // Code from Dr. James McCaffrey in https://quaetrix.com/Matrix/code.html

            int n = matrix.GetLength(0);
            double[][] m = MatrixRebuild(matrix);
            double[][] result = MatrixCreate(n, n);

            double[][] lum; // combined lower & upper
            int[] perm;
            int toggle;
            toggle = MatrixDecompose(m, out lum, out perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Helper(lum, b); // 
                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return MatrixRebuild(result);
        }

        /// <summary>
        /// Doolittle LUP decomposition with partial pivoting for solving linear equations.
        /// </summary>
        /// <param name="m">The matrix to decompose.</param>
        /// <param name="lum">The combined lower (dummy 1.0s in diagonal) & upper.</param>
        /// <param name="perm">Holds row permutations (P).</param>
        /// <returns>Return +1 or -1 according to even or odd number of row permutations.</returns>
        private static int MatrixDecompose(double[][] m, out double[][] lum, out int[] perm)
        {
            // Code from Dr. James McCaffrey in https://quaetrix.com/Matrix/code.html
            // Crout's LU decomposition for matrix determinant and inverse


            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.Length;

            // make a copy of m[][] into result lum[][]
            lum = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i][j] = m[i][j];


            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j][j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    double[] tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }

            } // j

            return toggle;
        }

        /// <summary>
        /// Provides the solution to L x = b.
        /// </summary>
        /// <param name="luMatrix">Lower-triangular matrix.</param>
        /// <param name="b">Vector of the same leading dimension as L.</param>
        /// <returns>The transpose.</returns>
        private static double[] Helper(double[][] luMatrix, double[] b) // helper
        {
            // Code from Dr. James McCaffrey in https://quaetrix.com/Matrix/code.html
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }
    }
}
