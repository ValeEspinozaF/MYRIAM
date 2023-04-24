using DataStructures;
using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.Utils;
using static GeodesicFunctions.GeodesicDistance;
using Cartography;

namespace MYRIAM
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
            double score = Point_vsPointArray(coord, contour).Min(double.NaN) / buffer;
            return score >= 1 ? 1 : score;
        }
    }
}
