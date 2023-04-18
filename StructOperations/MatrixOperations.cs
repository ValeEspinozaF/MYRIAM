using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructOperations
{
    class MatrixOperations
    {
        /// <summary>
        /// Creates an empty matrix instance of the given rows and cols.
        /// </summary>
        /// <param name="rows">Amount of rows for the matrix.</param>
        /// <param name="cols">Amount of columns for the matrix.</param>
        /// <returns>Empty matrix of rows x cols dimensions.</returns>
        public static double[,] MatrixCreate(int rows, int cols)
        {
            double[,] result = new double[rows,cols];

            return result;
        }


        /// <summary>
        /// Checks whether the given matrix is an identity matrix.
        /// </summary>
        /// <param name="matrix">The matrix to evaluate.</param>
        /// <returns>True if matrix is an identity matrix, false otherwise.</returns>
        public static bool Matrix_IsIdentity(double[,] matrix)
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
        public static bool Matrix_IsNaN(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (!double.IsNaN(matrix[i,j]))
                        return false;
                }
            
            return true;
        }

        /// <summary>
        /// Generates an identity matrix of side n.
        /// </summary>
        /// <param name="matrix">The matrix to transpose.</param>
        /// <returns>A n x n identity matrix.</returns>
        public static double[,] MatrixIdentity(int n)
        {
            double[,] result = MatrixCreate(n, n);

            for (int i = 0; i < n; ++i)
                result[i,i] = 1.0;

            return result;
        }


        /// <summary>
        /// Calculates the dot-product between two matrices.
        /// </summary>
        /// <param name="matrixA">First matrix.</param>
        /// <param name="matrixB">Second matrix.</param>
        /// <returns>The result from matrixA * matrix B.</returns>
        public static double[,] MatrixDotProduct(double[,] matrixA, double[,] matrixB)
        {
            int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
            int bRows = matrixB.GetLength(0); int bCols = matrixB.GetLength(1);
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");

            double[,] result = MatrixCreate(aRows, bCols);

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
        public static T[,] MatrixTranspose<T>(T[,] matrix)
        {
            var result = new T[matrix.GetLength(1), matrix.GetLength(0)];

            for (int j = 0; j < matrix.GetLength(1); j++)
                for (int i = 0; i < matrix.GetLength(0); i++)
                    result[j, i] = matrix[i, j];

            return result;
        }


        /// <summary>
        /// Calculates the inverse of the given square matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverse of a square matrix.</returns>
        public static double[,] MatrixInverse(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = MatrixDuplicate(matrix);

            int[] permut;
            double[,] LUP = MatrixDecompose(matrix, out permut);
            if (LUP == null)
                throw new Exception("Unable to compute inverse");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                // P * Identity
                for (int j = 0; j < n; ++j)
                {
                    if (i == permut[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(LUP, b);

                for (int j = 0; j < n; ++j)
                    result[j, i] = x[j];
            }
            return result;
        }


        /// <summary>
        /// Creates a duplicate of a matrix.
        /// </summary>
        /// <param name="matrix">The matrix to duplicate.</param>
        /// <returns>A matrix duplicate.</returns>
        public static double[,] MatrixDuplicate(double[,] matrix)
        {
            double[,] result = MatrixCreate(matrix.GetLength(0), matrix.GetLength(1));

            for (int i = 0; i < matrix.GetLength(0); ++i) 
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    result[i, j] = matrix[i, j];

            return result;
        }


        /// <summary>
        /// Provides the solution to L x = b.
        /// </summary>
        /// <param name="luMatrix">Lower-triangular matrix.</param>
        /// <param name="b">Vector of the same leading dimension as L.</param>
        /// <returns>The transpose.</returns>
        public static double[] HelperSolve(double[,] luMatrix, double[] b)
        {
            int n = luMatrix.GetLength(0);
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum / luMatrix[i, i];
            }
            return x;
        }


        /// <summary>
        /// Doolittle LUP decomposition with partial pivoting for solving linear equations.
        /// </summary>
        /// <param name="matrix">The matrix to decompose.</param>
        /// <param name="permut">Holds row permutations (P).</param>
        /// <returns>L (with 1s on diagonal) and U.</returns>
        public static double[,] MatrixDecompose(double[,] matrix, out int[] permut)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; 

            double[,] result = MatrixDuplicate(matrix);

            // Track row permutations
            permut = new int[n];
            for (int i = 0; i < n; ++i) { permut[i] = i; }

            // Track row swaps (+1 for even rows, -1 for odd ones)
            int toggle = 1;

            for (int j = 0; j < n - 1; ++j) 
            {
                // Find largest values in column
                double colMax = Math.Abs(result[j, j]); 
                int pRow = j;

                for (int i = j + 1; i < n; ++i)
                {
                    if (Math.Abs(result[i, j]) > colMax)
                    {
                        colMax = Math.Abs(result[i, j]);
                        pRow = i;
                    }
                }

                // If largest value is not on pivot, swap rows
                if (pRow != j) 
                {
                    double[] rowPtr = ArrayManagement.GetRow(result, pRow);
                    for (int g = 0; g < rowPtr.Length; g++)
                    {
                        result[pRow, g] = result[j, g];
                        result[j, g] = rowPtr[g];
                    }

                    // Swap permutation info
                    (permut[j], permut[pRow]) = (permut[pRow], permut[j]);

                    // Adjust the row-swap toggle
                    toggle = -toggle; 
                }

                for (int i = j + 1; i < n; ++i)
                {
                    result[i, j] /= result[j, j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i, k] -= result[i, j] * result[j, k];
                    }
                }
            } 
            return result;
        }
    }
}