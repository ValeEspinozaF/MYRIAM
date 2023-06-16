using DataStructures;
using GeographicLib.Geocodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace Cartography
{
    public enum CoordinateUnit
    {
        Degrees = 1,
        Radians = 2,
    }

    public class Coordinate : PositionXY
    {
        /// <summary>
        /// Geographic Longitude in degrees.
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// Geographic Latitude in degrees.
        /// </summary>
        public double Lat { get; set; }
        
        /// <summary>
        /// Scalar value assigned to this particular coordinate.
        /// </summary>
        public double Val { get; set; }

        /// <summary>
        ///     The unit this Coordinate was constructed with.
        /// </summary>
        [DataMember(Name = "Unit", Order = 1)]
        public readonly CoordinateUnit? _unit;


        /// <summary>
        /// Initializes a new instance of the Coordinate class with Lon and Lat properties.
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        public Coordinate(double lon, double lat, CoordinateUnit? unit) : base(lon, lat)
        {
            Lon = lon;
            Lat = lat;
            _unit = unit;
        }


        /// <summary>
        /// Initializes a new instance of the Coordinate class with Lon, Lat and Val properties.
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="val"></param>
        public Coordinate(double lon, double lat, double val, CoordinateUnit unit) : base(lon, lat)
        {
            Lon = lon;
            Lat = lat;
            Val = val;
            _unit = unit;
        }

        

        #region Static Factory Methods

        public static Coordinate FromUnit(double longitude, double latitude, CoordinateUnit unit)
        {
            switch (unit)
            {
                case CoordinateUnit.Degrees:
                    return FromDegrees(longitude, latitude);
                case CoordinateUnit.Radians:
                    return FromRadians(longitude, latitude);
                default:
                    throw new ArgumentException("Invalid coordinate unit.");
            }
        }

        /// <summary>
        /// Creates a <see cref="Coordinate"/> from <see cref="CoordinateUnit.Degrees"/>.
        /// </summary>
        /// <param name="lon">Geographic longitude in degrees.</param>
        /// <param name="lat">Geographic latitude in degrees.</param>
        /// <returns></returns>
        public static Coordinate FromDegrees(double lon, double lat)
        {
            return new Coordinate(lon, lat, CoordinateUnit.Degrees);
        }

        /// <summary>
        /// Creates a <see cref="Coordinate"/> from <see cref="CoordinateUnit.Radians"/>.
        /// </summary>
        /// <param name="lon">Geographic longitude in radians.</param>
        /// <param name="lat">Geographic longitude in radians.</param>
        /// <returns></returns>
        public static Coordinate FromRadians(double lon, double lat)
        {
            return new Coordinate(lon, lat, CoordinateUnit.Radians);
        }
        #endregion


        #region Inner Conversion Methods

        /// <summary>
        /// Convert a vector 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Coordinate ToSpherical(Vector vector)
        {
            vector.ToSpherical();
            return new Coordinate(vector.Longitude, vector.Latitude, vector.Magnitude, CoordinateUnit.Degrees);
        }

        public Coordinate ToDegres()
        {
            var lon = this.Lon * (Math.PI / 180);
            var lat = (90 - this.Lat) * (Math.PI / 180);

            return new Coordinate(lon, lat, CoordinateUnit.Degrees);
        }

        public Coordinate ToRadians()
        {
            var lon = this.Lon * (Math.PI / 180);
            var lat = (90 - this.Lat) * (Math.PI / 180);

            return new Coordinate(lon, lat, CoordinateUnit.Radians);
        }

        public static Coordinate[] ToRadians(Coordinate[] coordsDeg)
        {
            Coordinate[] coordsRad = new Coordinate[coordsDeg.Length];
            for (int i = 0; i < coordsDeg.Length; i++)
            {
                var lon = coordsDeg[i].Lon * (Math.PI / 180);
                var lat = (90 - coordsDeg[i].Lat) * (Math.PI / 180);

                coordsRad[i] = new Coordinate(lon, lat, CoordinateUnit.Radians);
            }
            return coordsRad;
        }

        #endregion


        #region Vector Conversion Methods

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
        #endregion


        #region Other Methods
        public static double[,] ToArray(Coordinate[] coords, double[] val)
        {
            if (coords.Length != val.Length)
                throw new ArgumentException(
                    "Error! " +
                    "Coords and val array must be the same length."
                    );

            double[,] array = new double[coords.Length, 3];

            for (int i = 0; i < coords.Length; i++)
            {
                array[i, 0] = coords[i].Lon;
                array[i, 1] = coords[i].Lat;
                array[i, 2] = val[i];
            }
            return array;
        }

        public static Coordinate[] MakeGrid(Coordinate[] coordsArray, double stepDeg)
        {
            List<Coordinate> gridPoints = new();
            CoordinateUnit unit = (CoordinateUnit) coordsArray[0]._unit;

            // Get contour latlon limits
            CartoLimits cntrLim = CartoLimits.Extract_CoordsLimits(coordsArray);

            var lonStart = Math.Floor(cntrLim.LonMin); 
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
                    gridPoints.Add(Coordinate.FromUnit(lonRange[i], latRange[j], unit));
                }

            // Turn list into array
            return gridPoints.ToArray();
        }
        #endregion
    }
}
