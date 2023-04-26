using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using static StructOperations.ArrayOperations;
using static StructOperations.ArrayManagement;
using Utilities;
using StructOperations;

namespace DataStructures
{
    public partial class EulerVector
    {
        public static EulerVector[] Generate_Ensemble(double[] evArray)
        {
            // Build Euler Vector struct
            EulerVector euVector = new EulerVector();


            // Data without covariance
            if (evArray.Length <= 4)
            {
                // Covariance in rad2 / Myr2
                double variance = Math.Pow(0.05 * evArray[2], 2);
                double[] covArray = ArrayMultiply(new double[] { variance, 0, 0, variance, 0, variance }, Math.Pow(Math.PI / 180, 2));

                euVector.Longitude = evArray[0];
                euVector.Latitude = evArray[1];
                euVector.Magnitude = evArray[2];
                euVector.Covariance = new Covariance(covArray);
            }


            // Data with covariance
            else if (evArray.Length >= 9)
            {
                euVector.Longitude = evArray[0];
                euVector.Latitude = evArray[1];
                euVector.Magnitude = evArray[2];
                euVector.Covariance = new Covariance(SubArray(evArray, 3, 6));
            }


            // Set ensemble size
            int nSize = 1000000;
            if (evArray.Length == 4 || evArray.Length == 10)
                nSize = (int)evArray.Last();


            // Generate ensemble
            return Generate_Ensemble(euVector, nSize);
        }


        public static EulerVector[] Generate_Ensemble(EulerVector eulerVector, int nSize)
        {
            // Euler Vector from spherical deg to cartesian coordinate
            eulerVector.ToCartesian();


            // Covariance elements from rad2/Myr2 to deg2/Myr2
            Covariance covArray = new (ArrayMultiply(eulerVector.Covariance.Values(), Math.Pow(180/Math.PI, 2)));


            // Turn array to covariance matrix
            double[,] covMatrix = covArray.ToMatrix();


            // Generate correlation ensemble
            EulerVector[] CorrelatedEnsemble = CorrelatedEnsemble3D(covMatrix, nSize);


            // Add correlated ensemble to original Euler vector
            EulerVector[] EuVector_Ensemble = EulerVector.VectorSummate(CorrelatedEnsemble, eulerVector);


            return EuVector_Ensemble;
        }


        /// <summary>
        /// Builds an ensemble of cartesian coordinates of length
        /// <paramref name="nSize"/>, using the covariance matrix
        /// <paramref name="covMatrix"/> as correlation. 
        /// </summary>
        /// <param name="covMatrix">Covariance matrix.</param>
        /// <param name="nSize">Length of the output ensemble.</param>
        /// <returns>Array of cartesian coordinates.</returns>
        public static EulerVector[] CorrelatedEnsemble3D(double[,] covMatrix, int nSize)
        {
            // Store matrix as MathNet matrix class
            Matrix<double> matrix = Matrix<double>.Build.DenseOfArray(covMatrix);


            // Compute matriz EVD decomposition
            Evd<double> eigen = matrix.Evd();


            // Gets the eigen values and vectors
            double[] eigenValues = eigen.EigenValues.Real().ToArray();
            double[,] eigenVector = eigen.EigenVectors.ToArray();

            double[] sqrdVa = new double[3] 
            { 
                Math.Sqrt(Math.Abs(eigenValues[0])), 
                Math.Sqrt(Math.Abs(eigenValues[1])), 
                Math.Sqrt(Math.Abs(eigenValues[2])) 
            };


            // Build ensemble
            EulerVector[] CorrelatedEnsemble = new EulerVector[nSize];
            double[,] rndArray = Utils.RandomNormal2D(nSize, 3);

            for (int i = 0; i < nSize; i++)
            {
                double[] sqrdVaProd = ArrayProduct(sqrdVa, GetRow(rndArray, i));
                double[,] sqrdVaVector = Turn1DTo2D(sqrdVaProd, 0);
                double[,] eigenProduct = Matrix.DotProduct(eigenVector, sqrdVaVector);

                CorrelatedEnsemble[i] = new EulerVector
                {
                    X = eigenProduct[0, 0],
                    Y = eigenProduct[1, 0],
                    Z = eigenProduct[2, 0]
                };
            }

            return CorrelatedEnsemble;
        }
    }
}
