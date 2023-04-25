using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cartography
{
    public static class DeformationArea
    {
        public static double[] DeformationGridScore(Coordinate[] coordArray, Coordinate[] contour, double buffer)
        {
            double[] bufferScore = new double[coordArray.Length];

            int i = 0;
            foreach (Coordinate coord in coordArray)
            {
                bufferScore[i] = DeformationPointScore(coord, contour, buffer);
                i++;
            }

            return bufferScore;
        }

        private static double DeformationPointScore(Coordinate coord, Coordinate[] contour, double buffer)
        {
            double score = GeodesicDistance.Point_vsPointArray(coord, contour).Min(double.NaN) / buffer;
            return score >= 1 ? 1 : score;
        }

        private static T? Min<T>(this T[] arr, T ignore) where T : IComparable
        {
            bool minSet = false;
            T min = default;

            for (int i = 0; i < arr.Length; i++)
                if (arr[i].CompareTo(ignore) != 0)
                    if (!minSet)
                    {
                        minSet = true;
                        min = arr[i];
                    }
                    else if (arr[i].CompareTo(min) < 0)
                        min = arr[i];

            return minSet ? min : ignore;
        }
    }
}
