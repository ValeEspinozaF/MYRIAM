﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using Cartography;


namespace GeodesicFunctions
{
    class GeodesicDistance
    {
        // Earth's radius in km
        const double Re = 6371;

        public static double[] Point_vsPointArray(Coordinate point, Coordinate[] coordArray)
        {
            // Turn input coordinates from spherical to radians
            Vector v1 = point.ToCartesian(Re);
            Vector[] va2 = coordArray.Select(x => x.ToCartesian(Re)).ToArray();

            double[] g_dst = new double[coordArray.Length];

            for (int i = 0; i < coordArray.Length; i++)
            {
                double dot_p = v1.X * va2[i].X + v1.Y * va2[i].Y + v1.Z * va2[i].Z;
                g_dst[i] = Re * Math.Acos(dot_p / Math.Pow(Re, 2)) * 1000; // in meters
            }
            return g_dst;
        }

        public static double[] PointArray(Coordinate[] coordArray)
        {
            // Turn input coordinates from spherical to cartesian [radians]
            Vector[] cv = coordArray.Select(x => x.ToCartesian(Re)).ToArray();

            // Output geodesic distance
            double[] g_dst = new double[coordArray.Length];

            for (int i = 0; i < coordArray.Length -1; i++)
            {
                Vector v1 = cv[i];
                Vector v2 = cv[i + 1];

                double dot_p = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
                g_dst[i] = Re * Math.Acos(dot_p / Math.Pow(Re, 2)) * 1000; // in meters
            }
            return g_dst;
        }
    }
}
