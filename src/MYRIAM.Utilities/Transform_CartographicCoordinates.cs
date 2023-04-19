﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using DataStructures;


namespace CartographicCoordinates
{
    class TransformSystem
    {
        // Degrees to Degrees
        public static double ToRadians(double value)
        {
            return value * (Math.PI / 180);
        }

        public static Coord[] DegToRad(Coord[] points)
        {
            var pointsRad = new Coord[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                pointsRad[i] = new Coord
                {
                    X = ToRadians(points[i].X),
                    Y = ToRadians(90 - points[i].Y)
                };
            }
            return pointsRad;
        }



        // Radians to Degrees
        public static double ToDegrees(double value)
        {
            return value * (180 / Math.PI);
        }
        
        public static Coord[] RadToDeg(Coord[] points)
        {
            var pointsDeg = new Coord[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                pointsDeg[i] = new Coord
                {
                    X = ToDegrees(points[i].X),
                    Y = 90 - ToDegrees(points[i].Y)
                };
            }
            return pointsDeg;
        }



        // Radians to Cartesian
        public static vectorCart RadToCart(double lon, double lat, double angle = 1)
        {
            return new vectorCart
            {
                X = angle * Math.Cos(lon) * Math.Cos(lat),
                Y = angle * Math.Cos(lat) * Math.Sin(lon),
                Z = angle * Math.Sin(lat)
            };
        }

        public static vectorCart RadToCart(vectorSph vectorSph)
        {
            double lon = vectorSph.Longitude;
            double lat = vectorSph.Latitude;
            double ang = vectorSph.Angle;

            return RadToCart(lon, lat, ang);
        }

        public static vectorCart[] RadToCart(vectorSph[] vectorsSph)
        {
            vectorCart[] vectorsCart = new vectorCart[vectorsSph.Length];

            for (int i = 0; i < vectorsSph.Length; i++)
                vectorsCart[i] = RadToCart(vectorsSph[i]);

            return vectorsCart;
        }

        public static Coord[] RadToCart(Coord[] points, double[]? z = null)
        {
            var pointsCart = new Coord[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                double lon = points[i].X;
                double lat = (Math.PI / 2) - points[i].Y;
                double angle = z != null ? z[i] : 1;

                pointsCart[i] = new Coord
                {
                    X = angle * Math.Cos(lon) * Math.Cos(lat),
                    Y = angle * Math.Cos(lat) * Math.Sin(lon),
                    Z = angle * Math.Sin(lat)
                };
            }
            return pointsCart;
        }



        // Degrees to Cartesian
        public static vectorCart DegToCart(vectorSph vectorSph)
        {
            double lon = ToRadians(vectorSph.Longitude);
            double lat = ToRadians(vectorSph.Latitude);
            double ang = vectorSph.Angle;

            return RadToCart(lon, lat, ang);
        }

        public static vectorCart[] DegToCart(vectorSph[] vectorsSph)
        {
            vectorCart[] vectorsCart = new vectorCart[vectorsSph.Length];

            for (int i = 0; i < vectorsSph.Length; i++)
                vectorsCart[i] = DegToCart(vectorsSph[i]);

            return vectorsCart;
        }

        public static vectorCart DegToCart(Coord coord, double z = 1)
        {
            double lon = ToRadians(coord.X);
            double lat = ToRadians(coord.Y);

            return RadToCart(lon, lat, z);
        }

        public static vectorCart[] DegToCart(Coord[] coordsArray, double z = 1)
        {
            vectorCart[] vectorsCart = new vectorCart[coordsArray.Length];

            for (int i = 0; i < coordsArray.Length; i++)
                vectorsCart[i] = DegToCart(coordsArray[i], z);

            return vectorsCart;
        }



        // Cartesian to Radians
        public static vectorSph CartToRad(double x, double y, double z)
        {
            if (x == 0) x = double.Epsilon;

            return new vectorSph
            {
                Longitude = Math.Atan2(y, x),
                Latitude = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2))),
                Angle = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2)),
            };
        }

        public static vectorSph CartToRad(vectorCart vectorCart)
        {
            var x = vectorCart.X;
            var y = vectorCart.Y;
            var z = vectorCart.Z;

            return CartToRad(x, y, z);
        }

        public static vectorSph[] CartToRad(vectorCart[] vectorsCart)
        {
            vectorSph[] vectorsSph = new vectorSph[vectorsCart.Length];

            for (int i = 0; i < vectorsCart.Length; i++)
                vectorsSph[i] = CartToRad(vectorsCart[i]);

            return vectorsSph;
        }



        // Cartesian to Degrees
        public static vectorSph CartToDeg(vectorCart vectorCart)
        {
            vectorSph outSph = CartToRad(vectorCart);

            outSph.Longitude = ToDegrees(outSph.Longitude);
            outSph.Latitude = ToDegrees(outSph.Latitude);

            return outSph;
        }

        public static vectorSph[] CartToDeg(vectorCart[] vectorsCart)
        {
            vectorSph[] vectorsSph = new vectorSph[vectorsCart.Length];

            for (int i = 0; i < vectorsCart.Length; i++)
                vectorsSph[i] = CartToDeg(vectorsCart[i]);

            return vectorsSph;
        }

        public static void CartToDeg(vectorCart[] vectorsCart, out Coord[] coords)
        {
            coords = new Coord[vectorsCart.Length];

            for (int i = 0; i < vectorsCart.Length; i++)
            {
                vectorSph vectorSph = CartToDeg(vectorsCart[i]);
                coords[i] = new Coord()
                {
                    X = vectorSph.Longitude,
                    Y = vectorSph.Latitude
                };
            }
        }
    }
}
