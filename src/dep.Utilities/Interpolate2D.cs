﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StructOperations.ArrayManagement;
using DataStructures;
using Cartography;


namespace ComputationalGeometry
{
    class GridInterpolation
    { // Implemented only for rectangular equally spaced grids

        private delegate double MyInterpolation_Method(double[] x, double[] y, double[,] z, double xval, double yval);


        /// <summary>
        /// Interpolates using the nearest point whithin a recatangular grid of points.
        /// </summary>
        /// <param name="x">Array of Lon-coordinate values.</param>
        /// <param name="y">Array of Lat-coordinate values.</param>
        /// <param name="z">Grid of Val-values for each XY coordinate.</param>
        /// <param name="xval">Lon-coordinate of the query point.</param>
        /// <param name="yval">Lat-coordinate of the query point.</param>
        /// <returns>The Val-value of the queried point.</returns>
        public static double NearestInterpolation(double[] x, double[] y, double[,] z, double xval, double yval)
        {
            double closest_x = x.Aggregate((x1, x2) => Math.Abs(x1 - xval) < Math.Abs(x2 - xval) ? x1 : x2);
            double closest_y = y.Aggregate((y1, y2) => Math.Abs(y1 - yval) < Math.Abs(y2 - yval) ? y1 : y2);
            int index_x = Array.IndexOf(x, closest_x);
            int index_y = Array.IndexOf(y, closest_y);

            return z[index_x, index_y];
        }


        /// <summary>
        /// Interpolates linearly whithin a rectangular grid of points. 
        /// </summary>
        /// <param name="x">Array of Lon-coordinate values.</param>
        /// <param name="y">Array of Lat-coordinate values.</param>
        /// <param name="z">Grid of Val-values for each XY coordinate.</param>
        /// <param name="xval">Lon-coordinate of the query point.</param>
        /// <param name="yval">Lat-coordinate of the query point.</param>
        /// <returns>The Val-value of the queried point.</returns>
        public static double BilinearInterpolation(double[] x, double[] y, double[,] z, double xval, double yval)
        {
            int i = Array.BinarySearch(x, xval);
            int j = Array.BinarySearch(y, yval);

            if (i < 0)
                i = ~i - 1;
            if (j < 0)
                j = ~j - 1;

            return
                z[i, j] * (x[i + 1] - xval) * (y[j + 1] - yval) / (x[i + 1] - x[i]) / (y[j + 1] - y[j]) +
                z[i + 1, j] * (xval - x[i]) * (y[j + 1] - yval) / (x[i + 1] - x[i]) / (y[j + 1] - y[j]) +
                z[i, j + 1] * (x[i + 1] - xval) * (yval - y[j]) / (x[i + 1] - x[i]) / (y[j + 1] - y[j]) +
                z[i + 1, j + 1] * (xval - x[i]) * (yval - y[j]) / (x[i + 1] - x[i]) / (y[j + 1] - y[j]);
        }


        /// <summary>
        /// Interpolate over a 2-D grid.
        /// </summary>
        /// <param name="X">Grid of Lon-coordinate values.</param>
        /// <param name="Y">Grid of Lat-coordinate values.</param>
        /// <param name="V">Grid of Val-values for each xy coordinate.</param>
        /// <param name="XYqs">Coordinate array with points to interpolate.</param>
        /// <param name="method">Interpolation method. Only "linear" and "nearest" are supported.</param>
        /// <returns>Array of Val-values for the queried points.</returns>
        public static double[] Interpolation2D(double[,] X, double[,] Y, double[,] V, Coordinate[] XYqs, string method="linear")
        {
            double[,] Vt = Matrix.Transpose(V);
            double[,] Xt = Matrix.Transpose(X);
            double[,] Yt = Matrix.Transpose(Y);


            double[,] X_, Y_, V_;
            checkMonotonic(Xt, Yt, Vt, out X_, out Y_, out V_);


            double[] Xdata = GetColumn(X_, 0);
            double[] Ydata = GetRow(Y_, 0);
            double[] Zvals = new double[XYqs.Length];

            MyInterpolation_Method Interpolation_Method = null;
            if (method == "linear") Interpolation_Method = BilinearInterpolation;
            if (method == "nearest") Interpolation_Method = NearestInterpolation;
            
            return XYqs.Select(xyq => Interpolation_Method(Xdata, Ydata, V_, xyq.Lon, xyq.Lat)).ToArray();
        }


        private static void checkMonotonic(
            double[,] X, double[,] Y, double[,] V, 
            out double[,] X_, out double[,] Y_, out double[,] V_)
        {
            makeMonotonic(X, V, 0, out X_, out _);
            makeMonotonic(Y, V, 1, out Y_, out V_); 
        }


        private static void makeMonotonic(
            double[,] X, double[,] V, int axis, 
            out double[,] Xout, out double[,] Vout)
        {
            Xout = X;
            Vout = V;

            if (X.GetLength(axis) > 1)
            {
                int[] subArray = SubArray(GetSize(X), 0, axis);
                double[] Xf = ArrayFlatten(X, 1);

                var prod = 1;
                for (int i = 0; i < subArray.Length; i++)
                    prod *= subArray[i];

                if (Xf[0] > Xf[prod])
                {
                    Xout = Flip(X, axis);
                    Vout = Flip(V, axis);
                }
            }
        }
    }
}
