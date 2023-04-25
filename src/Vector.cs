using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataStructures
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Magnitude { get; set; }
        public Covariance Covariance { get; set; }

        public Vector()
        {
        }

        public virtual Vector ToCartesian()
        {
            this.X = this.Magnitude * Math.Cos(this.Longitude) * Math.Cos(this.Latitude);
            this.Y = this.Magnitude * Math.Cos(this.Latitude) * Math.Sin(this.Longitude);
            this.Z = this.Magnitude * Math.Sin(this.Latitude);

            return this;
        }

        public static Vector[] ToCartesian(Vector[] vectorArray)
        {
            for (int i = 0; i < vectorArray.Length; i++)
                vectorArray[i].ToCartesian();

            return vectorArray;
        }

        public virtual Vector ToSpherical() 
        {
            double x = this.X;
            double y = this.Y;
            double z = this.Z;

            if (x == 0) x = double.Epsilon;

            this.Longitude = Math.Atan2(y, x);
            this.Latitude = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            this.Magnitude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

            this.Longitude *= 180 / Math.PI;
            this.Latitude *= 180 / Math.PI;

            return this;
        }

        public static Vector[] ToSpherical(Vector[] vectorArray)
        {
            for (int i = 0; i < vectorArray.Length; i++)
                vectorArray[i].ToSpherical();

            return vectorArray;
        }

        public static void GetCartesianColumns(Vector[] vectorArray, out double[] X, out double[] Y, out double[] Z)
        {
            X = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].X).ToArray();
            Y = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Y).ToArray();
            Z = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Z).ToArray();
        }

        public static void GetSphericalColumns(Vector[] vectorArray, out double[] lon, out double[] lat, out double[] mag)
        {
            lon = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Longitude).ToArray();
            lat = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Latitude).ToArray();
            mag = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Magnitude).ToArray();
        }

        public static T[] VectorSummate<T>(T[] vectorArray, T vector) where T : Vector , new()      // !!! will this invecntion work?
        {
            T[] result = new T[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new T
                {
                    X = vectorArray[i].X + vector.X,
                    Y = vectorArray[i].Y + vector.Y,
                    Z = vectorArray[i].Z + vector.Z
                };
            }
            return result;
        }

        public static T[] VectorSubstract<T>(T[] arrayA, T[] arrayB) where T : Vector, new()
        {
            if (arrayA.Length != arrayB.Length)
                throw new ArgumentException("Arrays must have the same length.");

            T[] result = new T[arrayA.Length];

            for (int i = 0; i < arrayA.Length; i++)
            {
                result[i] = new T
                {
                    X = arrayA[i].X - arrayB[i].X,
                    Y = arrayA[i].Y - arrayB[i].Y,
                    Z = arrayA[i].Z - arrayB[i].Z
                };
            }
            return result;
        }

        public static T[] VectorMultiply<T>(T[] vectorArray, double value) where T : Vector, new()
        {
            T[] result = new T[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new T
                {
                    X = vectorArray[i].X * value,
                    Y = vectorArray[i].Y * value,
                    Z = vectorArray[i].Z * value
                };
            }
            return result;
        }

        public static T[] VectorProduct<T>(T[] vectorArray, double[,] matrix) where T : Vector, new()
        {
            T[] result = new T[vectorArray.Length];

            for (int i = 0; i < vectorArray.Length; i++)
            {
                result[i] = new T
                {
                    X = matrix[0, 0] * vectorArray[i].X + matrix[0, 1] * vectorArray[i].Y + matrix[0, 2] * vectorArray[i].Z,
                    Y = matrix[1, 0] * vectorArray[i].X + matrix[1, 1] * vectorArray[i].Y + matrix[1, 2] * vectorArray[i].Z,
                    Z = matrix[2, 0] * vectorArray[i].X + matrix[2, 1] * vectorArray[i].Y + matrix[2, 2] * vectorArray[i].Z
                };
            }
            return result;
        }
    }
}
