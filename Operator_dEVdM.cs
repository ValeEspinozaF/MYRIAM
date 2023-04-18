using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructOperations;
using DataStructures;


namespace MYRIAM
{
    class Operator_dEVdM
    {
        public static double[,] Build_MTX_w2M(double[] muAfHA, double[] dA, Coord[] cartArray)
        {
            // --- Matrix MTX_w2M (for MTX_w2M * w = M) ---

            // Replate negative values with zeros
            muAfHA = muAfHA.Select(x => x < 0 ? 0 : x).ToArray();


            // Set empty 3x3 matrix
            double[,] Matrix = new double[3, 3];


            // Calculate squared coordinates beforehand
            double[] Xsqrd = ( from vector in cartArray select Math.Pow(vector.X, 2) ).ToArray();
            double[] Ysqrd = ( from vector in cartArray select Math.Pow(vector.Y, 2)).ToArray();
            double[] Zsqrd = ( from vector in cartArray select Math.Pow(vector.Z, 2)).ToArray();


            for (int j = 0; j < muAfHA.Length; j++)
            {
                Matrix[0, 0] += muAfHA[j] * dA[j] * (Ysqrd[j] + Zsqrd[j]);
                Matrix[0, 1] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Y);
                Matrix[0, 2] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Z);
                Matrix[1, 0] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Y);
                Matrix[1, 1] += muAfHA[j] * dA[j] * (Xsqrd[j] + Zsqrd[j]);
                Matrix[1, 2] += muAfHA[j] * dA[j] * (cartArray[j].Y * cartArray[j].Z);
                Matrix[2, 0] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Z);
                Matrix[2, 1] += muAfHA[j] * dA[j] * (cartArray[j].Y * cartArray[j].Z);
                Matrix[2, 2] += muAfHA[j] * dA[j] * (Xsqrd[j] + Ysqrd[j]);
            }

            // Multiply times -1
            Matrix[0, 1] = -1.0 * Matrix[0, 1];
            Matrix[0, 2] = -1.0 * Matrix[0, 2];
            Matrix[1, 0] = -1.0 * Matrix[1, 0];
            Matrix[1, 2] = -1.0 * Matrix[1, 2];
            Matrix[2, 0] = -1.0 * Matrix[2, 0];
            Matrix[2, 1] = -1.0 * Matrix[2, 1];

            return Matrix;
        }


        public static double[,] Build_MTX_M2w(double[,] MTX_w2M)
        {
            // --- Matrix MTX_M2w (involved in w = MTX_M2w * M) ---
            double[,] MTX_M2w = MatrixOperations.MatrixInverse(MTX_w2M);

            return MTX_M2w;
        }
    }
}
