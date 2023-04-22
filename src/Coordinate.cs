using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var lonRange = Utils.Arange(lonStart, lonStop, stepDeg).ToArray();
            var latRange = Utils.Arange(latStart, latStop, stepDeg).ToArray();


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
