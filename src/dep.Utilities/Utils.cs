using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Utils
    {
        public static double[] Random1D(int nSamples)
        {
            Random random = new ();
            double[] values = new double[nSamples];

            for (int i = 0; i < nSamples; i++)
                values[i] = random.NextDouble();

            return values;
        }


        public static double[,] Random2D(int rows, int cols)
        {
            Random random = new ();
            double[,] values = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    values[i, j] = random.NextDouble();

            return values;
        }


        public static double[,] RandomNormal2D(int rows, int cols, double mean = 0, double stdDev = 1)
        {
            Random rand = new (); 
            double[,] values = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    double u1 = 1.0 - rand.NextDouble(); 
                    double u2 = 1.0 - rand.NextDouble();
                    double randStdNormal = 
                        Math.Sqrt(-2.0 * Math.Log(u1)) *
                        Math.Sin(2.0 * Math.PI * u2); 

                    values[i, j] = mean + stdDev * randStdNormal; 
                }

            return values;
        }


        public static IEnumerable<double> Range(double start, double stop, double step = 1)
        {
            if (step > 0) while (start < stop)
                {
                    yield return start;
                    start += step;
                }
            else while (start > stop)
                {
                    yield return start;
                    start += step;
                }
        }

        public static double[] Arange(double start, double stop, double step = 1)
        {
            int n = (int)Math.Ceiling((stop - start) / step);
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = start + i * step;
            }

            if (start + n * step <= stop)
                result = result.Concat(new double[] { start + n * step }).ToArray();

            return result;
        }

        public static double[] Linspace(double start, double stop, int nSamples)
        {
            double interval = (stop / Math.Abs(stop)) * Math.Abs(stop - start) / (nSamples - 1);

            return (from val in Enumerable.Range(0, nSamples)
                    select start + (val * interval)).ToArray();
        }


        public static bool CheckIncreasing(double[] array)
        {
            var y = array.First();
            return array.Skip(1).All(x =>
            {
                bool b = y.CompareTo(x) < 0;
                y = x;
                return b;
            });
        }


        public static bool CheckIncreasing(int[] array)
        {
            var y = array.First();
            return array.Skip(1).All(x =>
            {
                bool b = y.CompareTo(x) < 0;
                y = x;
                return b;
            });
        }


        public static double[] Difference(double[] array)
        {
            double[] diff = (from k in Enumerable.Range(0, array.Length - 1) select Math.Abs(array[k + 1] - array[k])).ToArray();

            return diff;
        }



    }
}
