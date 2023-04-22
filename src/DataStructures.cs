using CartographicCoordinates;
using ComputationalGeometry;



namespace DataStructures
{
    public struct Coord
    {
        public double X;
        public double Y;
        public double Z;

        public Coord(double x, double y, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool IsNaN()
        {
            if (double.IsNaN(this.X) && double.IsNaN(this.Y))
                return true;

            else
                return false;
        }
    }


    public struct CoordsLimits
    {
        public double LonMin;
        public double LonMax;
        public double LatMin;
        public double LatMax;

        public CoordsLimits(double lonMin, double lonMax, double latMin, double latMax)
        {
            LonMin = lonMin;
            LonMax = lonMax;
            LatMin = latMin;
            LatMax = latMax;
        }


        public CoordsLimits SetGlobal()
        {
            this =  new CoordsLimits 
            {
                LonMin = -180,
                LonMax = 180,
                LatMin = -90,
                LatMax = 90
            };

            return this;
        }

        public bool IsEqual(CoordsLimits coordsLimits)
        {
            return (
                this.LonMin == coordsLimits.LonMin &&
                this.LonMax == coordsLimits.LonMax &&
                this.LatMin == coordsLimits.LatMin &&
                this.LatMax == coordsLimits.LatMax
                );

        }

        public bool IsEmpty()
        {
            return (
                this.LonMin == 0 &&
                this.LonMax == 0 &&
                this.LatMin == 0 &&
                this.LatMax == 0
                );
        }

        public bool Enclose(Coord[] coords)
        {
            Coord[] rectangle = this.ToCoords();
            PolygonGeometry.Points_InPolygon(rectangle, coords);
            return coords.All(x => PolygonGeometry.Point_InPolygon(rectangle, x));
        }


        public double[] ToArray()
        {
            return new double[4] { LonMin, LonMax, LatMin, LatMax };
        }

        public Coord[] ToCoords()
        {
            return new Coord[4] {
                new Coord(LonMin, LatMin),
                new Coord(LonMax, LatMin),
                new Coord(LonMax, LatMax),
                new Coord(LonMin, LatMax)
            };
        }
    }


    public struct VectorDegCov
    {
        public double Longitude;
        public double Latitude;
        public double Angle;
        public Covariance Covariance;

        public VectorDegCov(double lon, double lat, double angle, Covariance cov)
        {
            Longitude = lon;
            Latitude = lat;
            Angle = angle;
            Covariance = cov;
        }

        public VectorDegCov(double lon, double lat, double angle)
        {
            Longitude = lon;
            Latitude = lat;
            Angle = angle;
            Covariance = new Covariance(Angle);
        }

        public VectorDegCov(VectorSph vector, Covariance cov)
        {
            Longitude = vector.Longitude;
            Latitude = vector.Latitude;
            Angle = vector.Angle;
            Covariance = cov;
        }

        public VectorDegCov(VectorSph vector)
        {
            Longitude = vector.Longitude;
            Latitude = vector.Latitude;
            Angle = vector.Angle;
            Covariance = new Covariance(Angle);
        }

        public VectorSph ToVectorSph()
        {
            return new VectorSph(Longitude, Latitude, Angle);
        }

        public double[] Values()
        {
            return new double[] { Longitude, Latitude, Angle,
                                  Covariance.C11, Covariance.C12, Covariance.C13,
                                  Covariance.C22, Covariance.C23, Covariance.C33 };
        }
    }


    public struct VectorCartCov
    {
        public double X;
        public double Y;
        public double Z;
        public Covariance Covariance;

        public VectorCartCov(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Covariance = new Covariance(TransformSystem.CartToRad(x, y, z).Angle);
        }

        public VectorCartCov(double x, double y, double z, Covariance cov)
        {
            X = x;
            Y = y;
            Z = z;
            Covariance = cov;
        }

        public double[] Values()
        {
            return new double[] { X, Y, Z,
                                  Covariance.C11, Covariance.C12, Covariance.C13,
                                  Covariance.C22, Covariance.C23, Covariance.C33 };
        }
    }
}
