using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using StructOperations;


namespace DataStructures
{
    public partial class EulerVector : Vector
    {
        public EulerVector()
        {
        }

        public override Vector ToCartesian() // spherical[degrees] to cartesian[radians]
        {
            double lon = this.Longitude * (Math.PI / 180);
            double lat = this.Latitude * (Math.PI / 180);
            double mag = this.Magnitude;

            this.X = mag * Math.Cos(lon) * Math.Cos(lat);
            this.Y = mag * Math.Cos(lat) * Math.Sin(lon);
            this.Z = mag * Math.Sin(lat);

            return this;
        }

        public static EulerVector[] ToCartesian(EulerVector[] vectorArray)
        {
            for (int i = 0; i < vectorArray.Length; i++)
                vectorArray[i].ToCartesian();

            return vectorArray;
        }

        public override Vector ToSpherical()  // cartesian[radians] to spherical[degrees]
        {
            double x = this.X;
            double y = this.Y;
            double z = this.Z;

            if (x == 0) x = double.Epsilon;

            this.Longitude = Math.Atan2(y, x);
            this.Latitude = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            this.Magnitude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

            // From rad to degrees
            this.Longitude = this.Longitude * (180 / Math.PI);
            this.Latitude = this.Latitude * (180 / Math.PI);

            return this;
        }

        public static EulerVector[] ToSpherical(EulerVector[] vectorArray)
        {
            for (int i = 0; i < vectorArray.Length; i++)
                vectorArray[i].ToSpherical();

            return vectorArray;
        }

        public static void GetCartesianColumns(EulerVector[] vectorArray, out double[] X, out double[] Y, out double[] Z)
        {
            X = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].X).ToArray();
            Y = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Y).ToArray();
            Z = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Z).ToArray();
        }

        public static void GetSphericalColumns(EulerVector[] vectorArray, out double[] lon, out double[] lat, out double[] mag)
        {
            lon = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Longitude).ToArray();
            lat = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Latitude).ToArray();
            mag = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Magnitude).ToArray();
        }

        public static EulerVector[] ArrayToCartesianVector(double[,] array2D)
        {
            EulerVector[] vectorArray = new EulerVector[array2D.GetLength(0)];

            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                double[] array = ArrayManagement.GetRow(array2D, i);
                vectorArray[i] = ArrayToCartesianVector(array);
            }

            return vectorArray;
        }


        public static EulerVector ArrayToCartesianVector(double[] array)
        {
            if (array.Length != 3)
                throw new ArgumentException("Array must have a length of three to turn it into a cartesian vector.");

            return new EulerVector{
                X = array[0], 
                Y = array[1], 
                Z = array[2] 
            };
        }
    }
}

