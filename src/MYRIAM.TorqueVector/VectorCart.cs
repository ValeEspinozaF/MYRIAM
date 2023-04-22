using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vector
{
    public class VectorCart
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Covariance Covariance { get; set; }

        public VectorCart()
        {
        }

        public VectorCart(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public VectorCart(double x, double y, double z, Covariance cov)
        {
            X = x;
            Y = y;
            Z = z;
            Covariance = cov;
        }

        public VectorCart(VectorCart vec, Covariance cov)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
            Covariance = cov;
        }
    }
}
