using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructOperations;
using Histograms;
using Utilities;


namespace MYRIAM
{
    internal class FNCT_PR09_F3_DATA_FIT
    {
        /// <summary>
        /// Best fit for the relationship between thickness and viscosity 
        /// contrast of the ashtenosphere, based on Paulson and Richards 2009.
        /// </summary>
        /// <param name="nSamples">Number of samples to build.</param>
        /// <returns>Ensemble of size <paramref name="nSamples"/> for values
        /// of A, the proportionality constant from Paulson and Richards 2009.
        /// </returns>
        public static double[] PR09_F3_DATA_FIT(int nSamples)
        {
            // Digitalized points from Paulson & Richards Fig3
            double[] x = new double[4] { 0.73346, 1.28794, 1.47276, 2.23476 };  // x = log10(muM / muA);
            double[] y = new double[4] { 271.739, 177.002, 153.661, 85.0114 };  // y = HA[km]
            double[] yl = new double[4] { 260.069, 166.018, 143.021, 72.6545 }; // yl = y's lower limit
            double[] yu = new double[4] { 282.037, 189.359, 166.018, 96.3387 }; // yu = y's upper limit


            // Log arrays
            double Log10(double item) => Math.Log10(item * 1e3);
            double[] yLog = ArrayOperations.ApplyFunction(y, Log10);
            double[] ylLog = ArrayOperations.ApplyFunction(yl, Log10);
            double[] yuLog = ArrayOperations.ApplyFunction(yu, Log10);


            int nPoints = x.Length;


            // Repeat Array x as nSamples-size columns in Matrix Mx 
            double[,] Mx = ArrayManagement.Repeat1DArray(x, 0, nSamples, 1);


            // Generate Matrix My from ylLog, yuLog and random values
            double[][] randomArray = ArrayManagement.ToArrayOfArrays(Utils.Random2D(nPoints, nSamples));
            double[][] My = new double[nPoints][];

            for (int i = 0; i < nPoints; i++)
            {
                My[i] = ArrayOperations.ArrayMultiply(randomArray[i], Math.Abs(ylLog[i] - yuLog[i]));
                My[i] = ArrayOperations.ArraySummate(My[i], Math.Min(ylLog[i], yuLog[i]));
            }


            // Generate Array Vq from the summation of Matrices My and 1/3Mx
            double[] Vq = new double[nSamples];
            for (int k1 = 0; k1 < nSamples; k1++)
            {
                double temp = 0;
                for (int j1 = 0; j1 < nPoints; j1++)
                    temp += My[j1][k1] + 1.0 / 3.0 * Mx[j1, k1];

                Vq[k1] = (double) 1 / nPoints * temp;
            }


            // Ensemble of A values
            double[] A_SAMPLES = (from value in Vq select Math.Pow(10, value)).ToArray();


            return A_SAMPLES;
        }
    }
}
