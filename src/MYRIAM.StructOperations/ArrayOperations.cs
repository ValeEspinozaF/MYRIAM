using System;
using System.Linq;

namespace StructOperations
{
    class ArrayOperations
    {
        public static T[] ApplyFunction<T>(T[] array, Func<T, T> function)
        {
            T[] result = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = function(array[i]);
            }

            return result;
        }

        public static double[] ApplyFunction<T>(T[] array, Func<T, double> function)
        {
            double[] result = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = function(array[i]);
            }

            return result;
        }

        public static double[] ApplyFunction(int[] array, Func<int, double> function)
        {
            double[] result = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = function(array[i]);
            }

            return result;
        }

        public static T[,] ApplyFunction<T>(T[] array1, T[] array2, Func<T, T, T> function)
        {
            T[,] result = new T[array1.Length, array2.Length];

            for (int i = 0; i < array1.Length; i++)
                for (int j = 0; j < array2.Length; j++)
                {
                    result[i, j] = function(array1[i], array2[j]);
                }

            return result;
        }

        public static double Product(double[] array)
        {
            int prod = 1;
            foreach (int value in array)
                prod *= value;

            return prod;
        }


        public static int Product(int[] array)
        {
            int prod = 1;
            foreach (int value in array)
                prod *= value;

            return prod;
        }


        public static int Sum(int[,] array)
        {
            int sum = 0;
            foreach (int value in array)
                sum += value;

            return sum;
        }


        public static double Sum(double[,] array)
        {
            double sum = 0;
            foreach (double value in array)
                sum += value;

            return sum;
        }

        public static double Sum(double[] array)
        {
            double sum = 0;
            foreach (double value in array)
                sum += value;

            return sum;
        }


        public static double[] ArrayProduct(double[] arrayA, double[] arrayB)
        {
            int aRows = arrayA.Length;
            int bRows = arrayB.Length;
            if (aRows != bRows)
                throw new Exception("Non-conformable arrays in ArrayProduct");

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; ++i)
                result[i] = arrayA[i] * arrayB[i];

            return result;
        }


        public static double[] ArrayProduct(double[] arrayA, int[] arrayB)
        {
            int aRows = arrayA.Length;
            int bRows = arrayB.Length;
            if (aRows != bRows)
                throw new Exception("Non-conformable arrays in ArrayProduct");

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; ++i)
                result[i] = arrayA[i] * arrayB[i];

            return result;
        }


        public static double[,] ArrayProduct(double[,] arrayA, int[,] arrayB)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);
            int bRows = arrayB.GetLength(0);
            int bCols = arrayB.GetLength(1);

            if (aRows != bRows || aCols != bCols)
                throw new Exception("Non-conformable arrays in ArrayProduct");

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] * arrayB[i, j];

            return result;
        }


        public static double[,] ArrayProduct(double[,] arrayA, double[,] arrayB)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);
            int bRows = arrayB.GetLength(0);
            int bCols = arrayB.GetLength(1);

            if (aRows != bRows || aCols != bCols)
                throw new Exception("Non-conformable arrays in ArrayProduct");

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] * arrayB[i, j];

            return result;
        }


        public static double[] ArrayMultiply(double[] arrayA, double value)
        {
            int aRows = arrayA.Length;

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; ++i)
                result[i] = arrayA[i] * value;

            return result;
        }


        public static double[,] ArrayMultiply(double[,] arrayA, double value)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i ,j] * value;

            return result;
        }


        public static double[] ArrayDivide(double[] arrayA, double value)
        {
            int aRows = arrayA.Length;

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; i++)
                result[i] = arrayA[i] / value;

            return result;
        }


        public static double[,] ArrayDivide(double[,] arrayA, double value)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] / value;

            return result;
        }


        public static double[,] ArrayDivide(double[,] arrayA, double[,] arrayB)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);
            int bRows = arrayB.GetLength(0);
            int bCols = arrayB.GetLength(1);

            if (aRows != bRows || aCols != bCols)
                throw new Exception("Non-conformable arrays in ArrayProduct");

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] / arrayB[i, j];

            return result;
        }


        public static double[] ArraySummate(double[] arrayA, double value)
        {
            double[] result = new double[arrayA.Length];

            for (int i = 0; i < arrayA.Length; i++)
                result[i] = arrayA[i] + value;

            return result;
        }

        public static double[,] ArraySummate(double[,] arrayA, double value)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] + value;

            return result;
        }

        public static double[,] ArraySummate(params double[][,] arrays)
        {
            int aRows = arrays[0].GetLength(0);
            int aCols = arrays[0].GetLength(1);

            double[,] result = ArrayManagement.Zeros2D(aRows, aCols, (double)0);

            foreach (var array in arrays)
            {
                for (int i = 0; i < aRows; i++)
                    for (int j = 0; j < aCols; j++)
                        result[i, j] += array[i, j];
            }
            return result;
        }


        public static double[] ArraySummate(params double[][] arrays)
        {
            int aRows = arrays[0].Length;

            double[] result = ArrayManagement.Zeros1D(aRows, (double)0);

            foreach (var array in arrays)
            {
                for (int i = 0; i < aRows; i++)
                        result[i] += array[i];
            }
            return result;
        }

        public static double[] ArraySubstract(double[] arrayA, double value)
        {
            int aRows = arrayA.Length;

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; ++i)
                result[i] = arrayA[i] - value;

            return result;
        }

        public static int[] ArraySubstract(int[] arrayA, int value)
        {
            int aRows = arrayA.Length;

            int[] result = new int[aRows];

            for (int i = 0; i < aRows; ++i)
                result[i] = arrayA[i] - value;

            return result;
        }

        public static double[] ArraySubstract(int[] arrayA, double value)
        {
            int aRows = arrayA.Length;

            double[] result = new double[aRows];

            for (int i = 0; i < aRows; ++i)
                result[i] = arrayA[i] - value;

            return result;
        }

        public static double[,] ArraySubstract(double[,] arrayA, double value)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] - value;

            return result;
        }

        public static double[,] ArraySubstract(int[,] arrayA, double value)
        {
            int aRows = arrayA.GetLength(0); 
            int aCols = arrayA.GetLength(1);

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i,j] = arrayA[i, j] - value;

            return result;
        }


        public static double[,] ArraySubstract(double[,] arrayA, double[,] arrayB)
        {
            int aRows = arrayA.GetLength(0);
            int aCols = arrayA.GetLength(1);
            int bRows = arrayB.GetLength(0);
            int bCols = arrayB.GetLength(1);

            if (aRows != bRows || aCols != bCols)
                throw new ArgumentException("Input arrays must have the same dimensions");

            double[,] result = new double[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                    result[i, j] = arrayA[i, j] - arrayB[i, j];

            return result;
        }


        public static double[] ArrayAbsolute(double[] array)
        {
            return (from i in Enumerable.Range(0, array.Length)
                    select Math.Abs(array[i])).ToArray();
        }
    }
}
