using DataStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using static CartographicCoordinates.TransformSystem;

namespace GeodesicFunctions
{
    class GeodesicDistance
    {
        // Earth's radius in km
        const double Re = 6371;

        public static double[] Point_vsPointArray(Coord point, Coord[] coordArray)
        {
            // Turn input coordinates from spherical to radians
            VectorCart v1 = DegToCart(point, Re);
            VectorCart[] va2 = DegToCart(coordArray, Re);

            double[] g_dst = new double[coordArray.Length];

            for (int i = 0; i < coordArray.Length; i++)
            {
                double dot_p = v1.X * va2[i].X + v1.Y * va2[i].Y + v1.Z * va2[i].Z;
                g_dst[i] = Re * Math.Acos(dot_p / Math.Pow(Re, 2)) * 1000; // in meters
            }
            return g_dst;
        }

        public static double[] PointArray(Coord[] coordArray)
        {
            // Turn input coordinates from spherical to cartesian [radians]
            VectorCart[] cv = DegToCart(coordArray, Re);

            // Output geodesic distance
            double[] g_dst = new double[coordArray.Length];

            for (int i = 0; i < coordArray.Length -1; i++)
            {
                VectorCart v1 = cv[i];
                VectorCart v2 = cv[i + 1];

                double dot_p = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
                g_dst[i] = Re * Math.Acos(dot_p / Math.Pow(Re, 2)) * 1000; // in meters
            }
            return g_dst;
        }
    }
}
