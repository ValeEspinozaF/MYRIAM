using DataStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Cartography
{
    public class Coordinate
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Val { get; set; }

        public Coordinate()
        {
        }

        public Coordinate(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
        }

        public Coordinate(double lon, double lat, double val)
        {
            Lon = lon;
            Lat = lat;
            Val = val;
        }


        public static double[,] ToArray(Coordinate[] coords, double[] z)
        {
            if (coords.Length != z.Length)
                throw new ArgumentException(
                    "Error! " +
                    "Coords and z array must be the same lenght."
                    );

            double[,] array = new double[coords.Length, 3];

            for (int i = 0; i < coords.Length; i++)
            {
                array[i, 0] = coords[i].Lon;
                array[i, 1] = coords[i].Lat;
                array[i, 2] = z[i];
            }
            return array;
        }

        public static Coordinate ToSpherical(Vector vector)
        {
            vector.ToSpherical();

            return new Coordinate
            {
                // radians to degrees
                Lon = vector.Longitude,
                Lat = vector.Latitude,
                Val = vector.Magnitude,
            };
        }
        public Vector ToCartesian(double? zVal = null)
        {
            // degrees to radians
            double lon = this.Lon * (Math.PI / 180);
            double lat = this.Lat * (Math.PI / 180);
            double val;

            if (this.Val != 0)
            {
                val = this.Val;
            }
            else
            {
                val = (double)(zVal != null ? zVal : 1);
            }

            double X = val * Math.Cos(lon) * Math.Cos(lat);
            double Y = val * Math.Cos(lat) * Math.Sin(lon);
            double Z = val * Math.Sin(lat);

            return new Vector{ 
                X = X, 
                Y = Y, 
                Z = Z 
            };
        }

        public static Vector[] ToCartesian(Coordinate[] coordDeg, double? zVal = null)
        {
            return coordDeg.Select(x => x.ToCartesian(zVal)).ToArray();
        }
        
        public Coordinate ToRadians()
        {
            this.Lon = this.Lon * (Math.PI / 180);
            this.Lat = (90 - this.Lat) * (Math.PI / 180);
            return this;
        }

        public static Coordinate[] ToRadians(Coordinate[] coordsDeg)
        {
            Coordinate[] coordsRad = new Coordinate[coordsDeg.Length];
            for (int i = 0; i < coordsDeg.Length; i++)
            {
                coordsRad[i] = new Coordinate()
                {
                    Lon = coordsDeg[i].Lon * (Math.PI / 180),
                    Lat = (90 - coordsDeg[i].Lat) * (Math.PI / 180),
                };
            }
            return coordsRad;
        }

        public static Coordinate[] MakeGrid(Coordinate[] coordsArray, double stepDeg)
        {
            List<Coordinate> gridPoints = new();

            // Get contour latlon limits
            CartoLimits cntrLim = CartoLimits.Extract_CoordsLimits(coordsArray);

            var lonStart = Math.Floor(cntrLim.LonMin);  // !!! May not be the best idea when plate is small right?
            var lonStop = Math.Ceiling(cntrLim.LonMax);
            var latStart = Math.Floor(cntrLim.LatMin);
            var latStop = Math.Ceiling(cntrLim.LatMax);


            // Generate latitude and longitude range 
            var lonRange = Utils.Arange(lonStart, lonStop, stepDeg);
            var latRange = Utils.Arange(latStart, latStop, stepDeg);


            // Fill matrix
            for (int i = 0; i < lonRange.Length; i++)
                for (int j = 0; j < latRange.Length; j++)
                {
                    gridPoints.Add(new Coordinate(lonRange[i], latRange[j]));
                }

            // Turn list into array
            return gridPoints.ToArray();
        }
    }
}
