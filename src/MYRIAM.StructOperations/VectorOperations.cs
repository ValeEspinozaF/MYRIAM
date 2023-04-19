using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;


namespace StructOperations
{
    class VectorOperations
    {
        public static vectorCart[] ArrayToCartesianVector(double[,] array2D)
        {
            vectorCart[] vectorArray = new vectorCart[array2D.GetLength(0)];

            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                double[] array = ArrayManagement.GetRow(array2D, i);
                vectorArray[i] = ArrayToCartesianVector(array);
            }

            return vectorArray;
        }


        public static vectorCart ArrayToCartesianVector(double[] array)
        {
            if (array.Length != 3)
                throw new ArgumentException("Array must have a length of three to turn it into a cartesian vector.");

            return new vectorCart
            {
                X = array[0],
                Y = array[1],
                Z = array[2]
            };
        }


        public static void GetVectorColumns(vectorCart[] vectorArray, out double[] X, out double[] Y, out double[] Z)
        {
            X = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].X).ToArray();
            Y = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Y).ToArray();
            Z = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Z).ToArray();
        }


        public static void GetVectorColumns(vectorSph[] vectorArray, out double[] X, out double[] Y, out double[] Z)
        {
            X = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Longitude).ToArray();
            Y = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Latitude).ToArray();
            Z = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Angle).ToArray();
        }


        public static void GetCovarianceColumns(Covariance[] cov, 
                                                out double[] C11, out double[] C12, out double[] C13,
                                                out double[] C22, out double[] C23, out double[] C33)
        {
            C11 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C11).ToArray();
            C12 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C12).ToArray();
            C13 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C13).ToArray();
            C22 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C22).ToArray();
            C23 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C23).ToArray();
            C33 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C33).ToArray();
        }


        public static vectorCart[] VectorSummate(vectorCart[] vectorArray, vectorCart vector)
        {
            vectorCart[] result = new vectorCart[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new vectorCart
                {
                    X = vectorArray[i].X + vector.X,
                    Y = vectorArray[i].Y + vector.Y,
                    Z = vectorArray[i].Z + vector.Z
                };
            }
            return result;
        }


        public static vectorCart[] VectorSubstract(vectorCart[] arrayA, vectorCart[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
                throw new ArgumentException("Arrays must have the same length.");

            vectorCart[] result = new vectorCart[arrayA.Length];

            for (int i = 0; i < arrayA.Length; i++)
            {
                result[i] = new vectorCart
                {
                    X = arrayA[i].X - arrayB[i].X,
                    Y = arrayA[i].Y - arrayB[i].Y,
                    Z = arrayA[i].Z - arrayB[i].Z
                };
            }
            return result;
        }


        public static vectorCart[] VectorMultiply(vectorCart[] vectorArray, double value)
        {
            vectorCart[] result = new vectorCart[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new vectorCart
                {
                    X = vectorArray[i].X * value,
                    Y = vectorArray[i].Y * value,
                    Z = vectorArray[i].Z * value
                };
            }
            return result;
        }


        public static vectorCart[] VectorProduct(vectorCart[] vectorArray, double[,] matrix)
        {
            vectorCart[] result = new vectorCart[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new vectorCart
                (
                    matrix[0, 0] * vectorArray[i].X + matrix[0, 1] * vectorArray[i].Y + matrix[0, 2] * vectorArray[i].Z,
                    matrix[1, 0] * vectorArray[i].X + matrix[1, 1] * vectorArray[i].Y + matrix[1, 2] * vectorArray[i].Z,
                    matrix[2, 0] * vectorArray[i].X + matrix[2, 1] * vectorArray[i].Y + matrix[2, 2] * vectorArray[i].Z
                );
            }
            return result;
        }
    }
}
