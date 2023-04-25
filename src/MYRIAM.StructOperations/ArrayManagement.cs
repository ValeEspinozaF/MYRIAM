using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StructOperations
{
    class ArrayManagement
    {
        public static int[] GetSize<T>(T[,] array)
        {
            return new int[] { array.GetLength(0), array.GetLength(1) };
        }

        public static T[] InsertbyIndex<T>(T[] array, T element, int index)
        {
            T[] newArray = new T[array.Length + 1];

            for (int i = 0; i < newArray.Length; i++)
            {
                if (i < index - 1)
                    newArray[i] = array[i];
                else if (i == index - 1)
                    newArray[i] = element;
                else
                    newArray[i] = array[i - 1];
            }
            return newArray;
        }

        /// <summary>
        /// Reshapes the given 1D <paramref name="array"/> into another
        /// 1D array of a given length (<paramref name="len"/>). 
        /// </summary>
        /// <remarks> If <paramref name="len"/> is smaller than the actual
        /// length of <paramref name="array"/>, then the output array will
        /// chop off the extra elements to the end of <paramref name="array"/>.
        /// If, on the other hand <paramref name="len"/> is larger than the
        /// length of <paramref name="array"/>, then the output array will
        /// feature extra slots filled with the input in <paramref name="empty"/>.
        /// </remarks>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="array">Array of samples.</param>
        /// <param name="empty">Input with which to fill any empty slots.</param>
        /// <param name="len">Length of the output array.</param>
        /// <param name="offset">Index of the first filled element in the output array.</param>
        /// <returns>Reshaped 1D array.</returns>
        public static T[] Reshape1D<T>(T[] array, T empty, int len, int offset = 0)
        {
            T[] reshaped = Repeat1D(empty, len);

            for (int i = 0; i < array.Length; i++)
                reshaped[i + offset] = array[i];

            return reshaped;
        }


        /// <summary>
        /// Reshapes the given 1D <paramref name="array"/> into a 2D array 
        /// of given dimensions. 
        /// </summary>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="array">1D array of samples.</param>
        /// <param name="dims">1x2 array where [0] is rows and [1] is columns.</param>
        /// <returns>Reshaped 2D array.</returns>
        public static T[,] Reshape2D<T>(T[] array, int[] dims)
        {
            T[,] reshaped = new T[dims[0], dims[1]];

            int k = 0;
            for (int i = 0; i < dims[0]; i++)
                for (int j = 0; j < dims[1]; j++)
                {
                    reshaped[i, j] = array[k];
                    k++;
                }
            return reshaped;
        }


        /// <summary>
        /// Reshapes the given 2D <paramref name="array"/> into another 
        /// 2D array of given dimensions. 
        /// </summary>
        /// <remarks>If dimensions only contains one element, this value will
        /// be taken as the number of rows and the other dimension will be calculated.
        /// </remarks>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="array">2D array of samples.</param>
        /// <param name="dims">New dimension of the 2D array.</param>
        /// <returns>Reshaped 2D array.</returns>
        public static T[,] Reshape2D<T>(T[,] array, int[] dims)
        {
            if (dims.Length > 2 || dims.Length == 0)
                throw new ArgumentException("Parameter dims must supply at least one and a maximun of two integers.");

            else if (dims.Length < 2)
            {
                double dimsCols = array.GetLength(0) * array.GetLength(1) / dims[0];

                if (dimsCols % 1 == 0)
                    dims = Reshape1D(dims, (int)dimsCols, 2);

                else
                    throw new ArgumentException($"Product of given rows ({dims[0]}) is not divisible into the total " +
                                                $"number of elements in array ({array.GetLength(0) * array.GetLength(1)}).");
            }

            else
                if (array.GetLength(0) * array.GetLength(1) != dims[0] * dims[1])
                    throw new ArgumentException($"Given output dimensions ({dims[0]}, {dims[1]}) cannot hold " +
                                                $"the total number of elements in array ({array.GetLength(0) * array.GetLength(1)}).");

            T[] flatArray = ArrayFlatten(array);
            T[,] reshaped = Reshape2D(flatArray, dims);

            return reshaped;
        }


        /// <summary>
        /// Reverses the order of elements in the given 1D array.
        /// </summary>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="array">1D array with elements to reverse.</param>
        /// <returns>1D array with reversed elements.</returns>
        public static T[] Flip<T>(T[] array)
        {
            return array.Reverse().ToArray();
        }


        /// <summary>
        /// Reverses the order of array, or elements whithin the arrays. 
        /// </summary>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="array">Array of arrays.</param>
        /// <param name="level">Level in which to perform the reversing, 
        /// [0] for arrays level, [1] for elements in array level.</param>
        /// <returns>Array of arrays with reversed array order or reversed elements whithin arrays.</returns>
        public static T[][] Flip<T>(T[][] array, int level = 0)
        {
            T[][] result = new T[array.Length][];

            for (int i = 0; i < array.Length; i++)
            {
                if (level == 0)
                    result[i] = array[array.Length - 1 - i];

                else if (level == 1)
                    result[i] = array[i].Reverse().ToArray();
            }

            return result;
        }


        /// <summary>
        /// Reverses the order of a given 2D array, along the given axis. 
        /// </summary>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="array">2D array of elements.</param>
        /// <param name="axis">Axis along which to perform the reversing, [0] to reverse row order, [1] to reverse columns.</param>
        /// <returns>Reversed 2D array along the given axis.</returns>
        public static T[,] Flip<T>(T[,] array, int axis)
        {
            int aRows = array.GetLength(0);
            int aCols = array.GetLength(1);

            T[,] result = new T[aRows, aCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < aCols; j++)
                {
                    if (axis == 0) result[aRows - 1 - i, j] = array[i, j];
                    else if (axis == 1) result[i, aCols - 1 - j] = array[i, j];
                }

            return result;
        }


        /// <summary>
        /// Turns array of arrays into a 2D array.
        /// </summary>
        /// <param name="arrays">Array or arrays. Each array will be a new row.</param>
        /// <returns>2D array with each input array as a new row.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double[,] To2DArray(params double[][] arrays)
        {
            foreach (var a in arrays)
            {
                if (a == null)
                    throw new ArgumentException("Can not contain null arrays");

                if (a.Length != arrays[0].Length)
                    throw new ArgumentException("Input arrays must have the same length");
            }

            var height = arrays[0].Length;
            var width = arrays.Length;

            var result = new double[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = arrays[j][i];
                }

            return result;
        }


        /// <summary>
        /// Flattens the elements in a 2D array into a 1D array. (if level=0, stacks rows).
        /// </summary>
        /// <typeparam name="T">Type of input in <paramref name="array"/>. 
        /// Output type will be the same.</typeparam>
        /// <param name="input"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static T[] ArrayFlatten<T>(T[,] input, int axis = 0)
        {
            if (axis == 1) input = MatrixOperations.MatrixTranspose(input);

            int size = input.Length;
            T[] result = new T[size];

            int write = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                for (int z = 0; z <= input.GetUpperBound(1); z++)
                {
                    result[write++] = input[i, z];
                }
            }
            return result;
        }


        public static T[] GetColumn<T>(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }


        public static T[] GetRow<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }


        public static double[,] Turn1DTo2D(double[] array, int axis)
        {
            var sizeResult = new int[2] { 1, 1 };
            sizeResult[axis] = array.Length;
            double[,] result = new double[sizeResult[0], sizeResult[1]];

            for (int i = 0; i < array.Length; i++)
            {
                if (axis == 0) result[i, 0] = array[i];
                if (axis == 1) result[0, i] = array[i];
            }
            return result;
        }


        public static double[,] Repeat1DArray(double[] array, int axis, int reps, int axisRep)
        {
            double[,] array2D = Turn1DTo2D(array, axis);

            return Repeat2DArray(array2D, reps, axisRep);
        }


        public static double[,] Repeat2DArray(double[,] array, int reps, int axis)
        {
            var size = new int[] { array.GetLength(0), array.GetLength(1) };

            var sizeReps = new int[2] { 1, 1 };
            sizeReps[axis] = reps;
            double[,] result = new double[size[0] * sizeReps[0], size[1] * sizeReps[1]];

            for (int k = 0; k < reps; k++)
            {
                for (int i = 0; i < size[0]; i++)
                    for (int j = 0; j < size[1]; j++)
                    {
                        if (axis == 0) result[i + size[0] * k, j] = array[i, j];
                        if (axis == 1) result[i, j + size[1] * k] = array[i, j];
                    }
            }
            return result;
        }


        public static T[] Repeat1D<T>(T value, int reps)
        {
            T[] result = new T[reps];

            for (int i = 0; i < reps; i++)
                result[i] = value;

            return result;
        }


        public static T[][] Repeat2D<T>(T[] array, int rows)
        {
            T[][] result = new T[rows][];

            for (int i = 0; i < rows; i++)
                result[i] = array;

            return result;
        }


        public static T[,] Repeat2D<T>(T value, int rows, int cols)
        {
            T[,] result = new T[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = value;

            return result;
        }


        public static double[] NaNs1D(int reps, double value = double.NaN)
        {
            return Repeat1D(value, reps);
        }


        public static double[] Zeros1D(int reps, double value = 0)
        {
            return Repeat1D(value, reps);
        }


        public static int[] Zeros1D(int reps, int value = 0)
        {
            return Repeat1D(value, reps);
        }


        public static double[,] Zeros2D(int rows, int cols, double value = 0)
        { 
            return Repeat2D(value, rows, cols);
        }


        public static int[,] Zeros2D(int rows, int cols, int value = 0)
        {
            return Repeat2D(value, rows, cols);
        }


        public static double[] Ones1D(int reps, double value = 1)
        {
            return Repeat1D(value, reps);
        }


        public static int[] Ones1D(int reps, int value = 1)
        {
            return Repeat1D(value, reps);
        }


        public static double[,] Ones2D(int rows, int cols, double value = 1)
        {
            return Repeat2D(value, rows, cols);
        }


        public static int[,] Ones2D(int rows, int cols, int value = 1)
        {
            return Repeat2D(value, rows, cols);
        }


        public static double[][] ToArrayOfArrays(params double[][] arrays)
        {
            double[][] arrayOfArrays = new double[arrays.GetLength(0)][];

            for (int i = 0; i < arrays.GetLength(0); i++)
            {
                arrayOfArrays[i] = arrays[i];
            }
            return arrayOfArrays;
        }

        public static double[][] ToArrayOfArrays(double[,] array)
        {
            double[][] arrayOfArrays = new double[array.GetLength(0)][];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                arrayOfArrays[i] = GetRow(array, i);
            }
            return arrayOfArrays;
        }


        public static int[][] ToArrayOfArrays(int[,] array)
        {
            int[][] arrayOfArrays = new int[array.GetLength(0)][];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                arrayOfArrays[i] = GetRow(array, i);
            }
            return arrayOfArrays;
        }

        public static double[] SubArrayLogic(double[] array, int[] logicArray)
        {
            if (array.Length != logicArray.Length)
                throw new Exception("Array and logic array must be the same length.");

            return array
                .Select((v, i) => new { v, i })
                .Where(x => logicArray[x.i] != 0)
                .Select(x => x.v)
                .ToArray();
        }

        public static double[] SubArrayIndexes(double[] array, int[] indexes)
        {
            return array
                .Select((v, i) => new { v, i })
                .Where(p => indexes.Contains(p.i))
                .Select(p => p.v)
                .ToArray();
        }

        public static double[] SubArray(double[] array, int offset, int length)
        {
            double[] result = new double[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }


        public static int[] SubArray(int[] array, int offset, int length)
        {
            int[] result = new int[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }


        public static double[,] SubArray2D(double[,] array, int offsetX, int lengthX, int offsetY, int lengthY)
        {
            double[,] result = new double[lengthX, lengthY];

            for (int i = 0; i < lengthY; i++)
            {
                for (int j = 0; j < lengthX; j++)
                {
                    result[i, j] = array[i + offsetY, j + offsetX];
                }
            }
            return result;
        }


        public static int[,] SubArray2D(int[,] array, int offsetX, int lengthX, int offsetY, int lengthY)
        {
            int[,] result = new int[lengthY, lengthX];

            for (int i = 0; i < lengthY; i++)
            {
                for (int j = 0; j < lengthX; j++)
                {
                    result[i, j] = array[i + offsetY, j + offsetX];
                }
            }
            return result;
        }

        public static bool IsZeros(double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    if (array[i, j] != 0)
                        return false;

            return true;
        }

        public static bool IsZeros(double[] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
                if (array[i] != 0)
                    return false;

            return true;
        }
    }
}
