using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using static CartographicCoordinates.TransformSystem;
using static StructOperations.MatrixOperations;
using MathNet.Numerics.LinearAlgebra;
using StructOperations;
using DataStructures;


namespace MYRIAM
{
    class Build_RotationMatrix
    {
        /// <summary>
        /// Given the rotation angles for each cartesian axis (in degrees),
        /// this function construct the three elemental rotations along 
        /// each axis, and multiplies them to return a general rotation matrix.
        /// </summary>
        /// <param name="z_angle">Displacement along the Z-axis (0E, 90N).</param>
        /// <param name="y_angle">Displacement along the Y-axis (90E, 0N).</param>
        /// <param name="x_angle">Displacement along the X-axis (0E, 0N).</param>
        /// <returns>Genereal rotation matrix.</returns>
        public static double[,] Set_RotationMatrix(double z_angle, double y_angle, double x_angle = 0)
        {
            // Turn vector to matrices
            double[,] RVCT_Z = VectorDeg_toMatrix(new vectorSph(0, 90, z_angle));
            double[,] RVCT_Y = VectorDeg_toMatrix(new vectorSph(90, 0, y_angle));
            double[,] RVCT_X = VectorDeg_toMatrix(new vectorSph(0,  0, x_angle));

            // Multiply rotation matrices
            return MatrixDotProduct( RVCT_X, MatrixDotProduct(RVCT_Y, RVCT_Z) );
        }


        /// <summary>
        /// Turns rotation vector into a rotation matrix.
        /// </summary>
        /// <param name="rotVector">Rotation vector in spherical coordinates.</param>
        /// <returns>Rotation matrix.</returns>
        public static double[,] VectorDeg_toMatrix(vectorSph rotVector)
        {
            // Convert angle from degrees to radians
            double lon = ToRadians(rotVector.Longitude);
            double lat = ToRadians(rotVector.Latitude);
            double ang = ToRadians(rotVector.Angle);

            double x = Math.Cos(lon) * Math.Cos(lat);
            double y = Math.Cos(lat) * Math.Sin(lon);
            double z = Math.Sin(lat);


            // Set output matrix
            double[,] matrix = new double[3, 3];
        

            // Convert vector to matrix
            matrix[0, 0] = Math.Cos(ang) + Math.Pow(x,2) * ( 1 - Math.Cos(ang) );
            matrix[0, 1] = x * y * ( 1 - Math.Cos(ang) ) - ( z * Math.Sin(ang) );
            matrix[0, 2] = x * z * ( 1 - Math.Cos(ang) ) + ( y * Math.Sin(ang) );
            matrix[1, 0] = y * x * ( 1 - Math.Cos(ang) ) + ( z * Math.Sin(ang) );
            matrix[1, 1] = Math.Cos(ang) + Math.Pow(y,2) * ( 1 - Math.Cos(ang) );
            matrix[1, 2] = y * z * ( 1 - Math.Cos(ang) ) - ( x * Math.Sin(ang) );
            matrix[2, 0] = z * x * ( 1 - Math.Cos(ang) ) - ( y * Math.Sin(ang) );
            matrix[2, 1] = z * y * ( 1 - Math.Cos(ang) ) + ( x * Math.Sin(ang) );
            matrix[2, 2] = Math.Cos(ang) + Math.Pow(z,2) * ( 1 - Math.Cos(ang) );

            return matrix;
        }
    }
}
