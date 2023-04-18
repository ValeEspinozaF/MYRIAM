using CartographicCoordinates;
using ContourBuilder;
using ComputationalGeometry;



namespace DataStructures
{
    public struct SphericalCoord
    {
        public double Longitude;
        public double Latitude;

        public SphericalCoord(double lon, double lat)
        {
            Longitude = lon;
            Latitude = lat;
        }
    }

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

    public struct vectorSph
    {
        public double Longitude;
        public double Latitude;
        public double Angle;

        public vectorSph(double lon, double lat, double ang)
        {
            Longitude = lon;
            Latitude = lat;
            Angle = ang;
        }

        public double[] Values()
        {
            return new double[] { Longitude, Latitude, Angle };
        }
    }

    public struct vectorCart
    {
        public double X;
        public double Y;
        public double Z;

        public vectorCart(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] Values()
        {
            return new double[] { X, Y, Z };
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

    public struct Covariance
    {
        public double C11;
        public double C12;
        public double C13;
        public double C22;
        public double C23;
        public double C33;

        public Covariance(double a, double b, double c, double d, double e, double f)
        {
            C11 = a;
            C12 = b;
            C13 = c;
            C22 = d;
            C23 = e;
            C33 = f;
        }

        public Covariance(double[] a_to_f)
        {
            C11 = a_to_f[0];
            C12 = a_to_f[1];
            C13 = a_to_f[2];
            C22 = a_to_f[3];
            C23 = a_to_f[4];
            C33 = a_to_f[5];
        }

        public Covariance(double angle)
        {
            double Diagonal = Math.Pow(0.1 * angle, 2); //!!! is it 10 the most reasonable?
            C11 = Diagonal;
            C12 = 0;
            C13 = 0;
            C22 = Diagonal;
            C23 = 0;
            C33 = Diagonal;
        }

        public double[] Values()
        {
            return new double[] { C11, C12, C13, C22, C23, C33 };
        }

        public double[,] ToMatrix()
        {
            return new double[3, 3]
            {
                { C11, C12, C13 },
                { C12, C22, C23 },
                { C13, C23, C33 }
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

        public VectorDegCov(vectorSph vector, Covariance cov)
        {
            Longitude = vector.Longitude;
            Latitude = vector.Latitude;
            Angle = vector.Angle;
            Covariance = cov;
        }

        public VectorDegCov(vectorSph vector)
        {
            Longitude = vector.Longitude;
            Latitude = vector.Latitude;
            Angle = vector.Angle;
            Covariance = new Covariance(Angle);
        }

        public vectorSph ToVectorSph()
        {
            return new vectorSph(Longitude, Latitude, Angle);
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


    public struct Contour
    {
        public int Length;
        public double Level;
        public double PercentInterval;
        public Coord[] Coordinates;

        public Contour(List<Isobar> isoBars, double level, double percentage)
        {
            Length = isoBars.Count;
            Level = level;
            PercentInterval = percentage;

            var CoordinatesList = new List<Coord>();
            foreach (var isobar in isoBars)
            {
                foreach (IsobarPoint point in isobar.Points)
                {
                    CoordinatesList.Add(new Coord(point.Location.X, point.Location.Y));
                }
                CoordinatesList.Add(new Coord(double.NaN, double.NaN));
            }

            Coordinates = CoordinatesList.ToArray();
            Coordinates = Coordinates.Take(Coordinates.Length - 1).ToArray();
        }
    }
}
