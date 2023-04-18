﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using GeographicLib;
using StructOperations;

namespace MYRIAM
{
    class DensifyPolygon
    {
        public static Coord[] DensifyPolygon_toDistance(Coord[] polygon, double step)
        {
            Coord[] dense_polygon = (Coord[])polygon.Clone();

            //Ellipsoid
            Geodesic ell = Geodesic.WGS84;

            // New points count
            int k = 0;
            for (int i = 1; i < polygon.Length; i++)
            {
                Coord v1 = polygon[i - 1];
                Coord v2 = polygon[i];

                // Line info
                var v1v2 = ell.Inverse(v1.Y, v1.X, v2.Y, v2.X);

                if (step < v1v2.Distance)
                {
                    int times = (int)Math.Floor(v1v2.Distance / step);

                    int index = i + k;
                    for (int j = 1; j <= times; j++)
                    {                    
                        // New point along line
                        var ph = ell.Direct(v1.Y, v1.X, v1v2.Azimuth1, step * j);
                        Coord vr = new() { X = ph.Longitude, Y = ph.Latitude };

                        // Update array with inserted element
                        dense_polygon = ArrayManagement.InsertbyIndex(dense_polygon, vr, index + j);

                        k++;
                    }
                }
            }
            return dense_polygon;
        }
    }
}
