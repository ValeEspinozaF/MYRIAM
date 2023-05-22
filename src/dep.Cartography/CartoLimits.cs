using ComputationalGeometry;
using ContourBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartography
{
    public class CartoLimits
    {
        public double LonMin { get; set; }
        public double LonMax { get; set; }
        public double LatMin { get; set; }
        public double LatMax { get; set; }

        public CartoLimits()
        {
        }

        public CartoLimits(double lonMin, double lonMax, double latMin, double latMax)
        {
            LonMin = lonMin;
            LonMax = lonMax;
            LatMin = latMin;
            LatMax = latMax;
        }


        public CartoLimits SetGlobal()
        {
            return new CartoLimits
            {
                LonMin = -180,
                LonMax = 180,
                LatMin = -90,
                LatMax = 90
            };
        }

        public override bool Equals(Object obj)
        {
            var other = obj as CartoLimits;
            return Equals(other);
        }

        public bool Equals(CartoLimits other)       // check if works
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (
                this.LonMin == other.LonMin &&
                this.LonMax == other.LonMax &&
                this.LatMin == other.LatMin &&
                this.LatMax == other.LatMax
                );
        }

        public bool IsEmpty()       // check if works
        {
            if (ReferenceEquals(null, this))
            {
                return true;
            }
            
            return (
                    this.LonMin == 0 &&
                    this.LonMax == 0 &&
                    this.LatMin == 0 &&
                    this.LatMax == 0
                    );
        }

        public bool Encloses(Coordinate[] coordinates)
        {
            Coordinate[] rectangle = this.ToCoords();

            return coordinates.All(x => PolygonGeometry.Point_InPolygon(rectangle, x));
        }


        public double[] ToArray()
        {
            return new double[4] { LonMin, LonMax, LatMin, LatMax };
        }

        public Coordinate[] ToCoords()
        {
            return new Coordinate[4] {
                Coordinate.FromDegrees(LonMin, LatMin),
                Coordinate.FromDegrees(LonMax, LatMin),
                Coordinate.FromDegrees(LonMax, LatMax),
                Coordinate.FromDegrees(LonMin, LatMax)
            };
        }

        public static CartoLimits Extract_CoordsLimits(Coordinate[] coords)
        {
            // Clone coordinates list
            var sortedList = new List<Coordinate>(coords.ToList());

            // Sort and extract limits
            sortedList.Sort((a, b) => a.Lon.CompareTo(b.Lon));
            double lonMin = sortedList[0].Lon;
            double lonMax = sortedList[^1].Lon;

            sortedList.Sort((a, b) => a.Lat.CompareTo(b.Lat));
            double latMin = sortedList[0].Lat;
            double latMax = sortedList[^1].Lat;

            var coordsLimits = new CartoLimits
            {
                LonMin = lonMin,
                LonMax = lonMax,
                LatMin = latMin,
                LatMax = latMax
            };

            return coordsLimits;
        }
    }
}
