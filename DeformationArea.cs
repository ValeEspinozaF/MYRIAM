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

namespace MYRIAM
{
    public static class DeformationArea
    {
        public static double[] DeformationGridScore(Coord[] coordArray, Coord[] contour, double buffer)
        {
            double[] bufferScore = new double[coordArray.Length];

            int i = 0;
            foreach (Coord coord in coordArray)
            {
                bufferScore[i] = DeformationPointScore(coord, contour, buffer);
                i++;
            }

            return bufferScore;
        }

        private static double DeformationPointScore(Coord coord, Coord[] contour, double buffer)
        {
            double score = Point_vsPointArray(coord, contour).Min(double.NaN) / buffer;
            return score >= 1 ? 1 : score;
        }
    }
}
