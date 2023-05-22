using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using GeographicLib;
using StructOperations;
using Cartography;

namespace MYRIAM
{
    public partial class PolygonGeometry
    {
        public static Coordinate[] DensifyPolygon_toDistance(Coordinate[] polygon, double step)
        {
            Coordinate[] dense_polygon = (Coordinate[])polygon.Clone();


            //Ellipsoid
            Geodesic ell = Geodesic.WGS84;

            // New points count
            int k = 0;
            for (int i = 1; i < polygon.Length; i++)
            {
                Coordinate v1 = polygon[i - 1];
                Coordinate v2 = polygon[i];

                // Line info
                var v1v2 = ell.Inverse(v1.Lat, v1.Lon, v2.Lat, v2.Lon);

                if (step < v1v2.Distance)
                {
                    int times = (int)Math.Floor(v1v2.Distance / step);

                    int index = i + k;
                    for (int j = 1; j <= times; j++)
                    {                    
                        // New point along line
                        var ph = ell.Direct(v1.Lat, v1.Lon, v1v2.Azimuth1, step * j);
                        Coordinate vr = Coordinate.FromDegrees(ph.Longitude, ph.Latitude);

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
