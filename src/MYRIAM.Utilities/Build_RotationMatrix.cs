using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using static StructOperations.MatrixOperations;


namespace MYRIAM
{
    class Build_RotationMatrix
    {
        /// <summary>
        /// Given the rotation angles for each cartesian axis (in degrees),
        /// this function construct the three elemental rotations along 
        /// each axis, and multiplies them to return a general rotation matrix.
        /// </summary>
        /// <param name="z_angle">Displacement along the Val-axis (0E, 90N).</param>
        /// <param name="y_angle">Displacement along the Lat-axis (90E, 0N).</param>
        /// <param name="x_angle">Displacement along the Lon-axis (0E, 0N).</param>
        /// <returns>Genereal rotation matrix.</returns>
        public static double[,] Set_RotationMatrix(double z_angle, double y_angle, double x_angle = 0)
        {
            // Turn vector to matrices
            double[,] RVCT_Z = VectorDeg_toMatrix(0, 90, z_angle);
            double[,] RVCT_Y = VectorDeg_toMatrix(90, 0, y_angle);
            double[,] RVCT_X = VectorDeg_toMatrix(0,  0, x_angle);

            // Multiply rotation matrices
            return MatrixDotProduct( RVCT_X, MatrixDotProduct(RVCT_Y, RVCT_Z) );
        }


        /// <summary>
        /// Turns rotation vector into a rotation matrix.
        /// </summary>
        /// <param name="rotVector">Rotation vector in spherical coordinates.</param>
        /// <returns>Rotation matrix.</returns>
        public static double[,] VectorDeg_toMatrix(double lon, double lat, double angle)
        {
            // Convert angle from degrees to radians
            double lonRad = lon * (Math.PI / 180);
            double latRad = lat * (Math.PI / 180);
            double angRad = angle * (Math.PI / 180);

            double x = Math.Cos(lonRad) * Math.Cos(latRad);
            double y = Math.Cos(latRad) * Math.Sin(lonRad);
            double z = Math.Sin(latRad);


            // Set output matrix
            double[,] matrix = new double[3, 3];
        

            // Convert vector to matrix
            matrix[0, 0] = Math.Cos(angRad) + Math.Pow(x,2) * ( 1 - Math.Cos(angRad) );
            matrix[0, 1] = x * y * ( 1 - Math.Cos(angRad) ) - ( z * Math.Sin(angRad) );
            matrix[0, 2] = x * z * ( 1 - Math.Cos(angRad) ) + ( y * Math.Sin(angRad) );
            matrix[1, 0] = y * x * ( 1 - Math.Cos(angRad) ) + ( z * Math.Sin(angRad) );
            matrix[1, 1] = Math.Cos(angRad) + Math.Pow(y,2) * ( 1 - Math.Cos(angRad) );
            matrix[1, 2] = y * z * ( 1 - Math.Cos(angRad) ) - ( x * Math.Sin(angRad) );
            matrix[2, 0] = z * x * ( 1 - Math.Cos(angRad) ) - ( y * Math.Sin(angRad) );
            matrix[2, 1] = z * y * ( 1 - Math.Cos(angRad) ) + ( x * Math.Sin(angRad) );
            matrix[2, 2] = Math.Cos(angRad) + Math.Pow(z,2) * ( 1 - Math.Cos(angRad) );

            return matrix;
        }
    }
}
