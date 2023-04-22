using DataStructures;
using StructOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EulerVector
{
    public class VectorCart
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Covariance Covariance { get; set; }

        public VectorCart()
        {
        }

        public VectorCart(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public VectorCart(double x, double y, double z, Covariance cov)
        {
            X = x;
            Y = y;
            Z = z;
            Covariance = cov;
        }

        public VectorCart(VectorCart vec, Covariance cov)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
            Covariance = cov;
        }

        public double[] Values()
        {
            return new double[] { X, Y, Z };
        }

        public static VectorCart[] ArrayToCartesianVector(double[,] array2D)
        {
            VectorCart[] vectorArray = new VectorCart[array2D.GetLength(0)];

            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                double[] array = ArrayManagement.GetRow(array2D, i);
                vectorArray[i] = ArrayToCartesianVector(array);
            }

            return vectorArray;
        }


        public static VectorCart ArrayToCartesianVector(double[] array)
        {
            if (array.Length != 3)
                throw new ArgumentException("Array must have a length of three to turn it into a cartesian vector.");

            return new VectorCart(array[0], array[1], array[2]);
        }


        public static void GetVectorColumns(VectorCart[] vectorArray, out double[] X, out double[] Y, out double[] Z)
        {
            X = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].X).ToArray();
            Y = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Y).ToArray();
            Z = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Z).ToArray();
        }

        public static VectorCart[] VectorSummate(VectorCart[] vectorArray, VectorCart vector)
        {
            VectorCart[] result = new VectorCart[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new VectorCart
                {
                    X = vectorArray[i].X + vector.X,
                    Y = vectorArray[i].Y + vector.Y,
                    Z = vectorArray[i].Z + vector.Z
                };
            }
            return result;
        }


        public static VectorCart[] VectorSubstract(VectorCart[] arrayA, VectorCart[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
                throw new ArgumentException("Arrays must have the same length.");

            VectorCart[] result = new VectorCart[arrayA.Length];

            for (int i = 0; i < arrayA.Length; i++)
            {
                result[i] = new VectorCart
                {
                    X = arrayA[i].X - arrayB[i].X,
                    Y = arrayA[i].Y - arrayB[i].Y,
                    Z = arrayA[i].Z - arrayB[i].Z
                };
            }
            return result;
        }


        public static VectorCart[] VectorMultiply(VectorCart[] vectorArray, double value)
        {
            VectorCart[] result = new VectorCart[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new VectorCart
                {
                    X = vectorArray[i].X * value,
                    Y = vectorArray[i].Y * value,
                    Z = vectorArray[i].Z * value
                };
            }
            return result;
        }

        public static VectorCart[] VectorProduct(VectorCart[] vectorArray, double[,] matrix)
        {
            VectorCart[] result = new VectorCart[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new VectorCart
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

