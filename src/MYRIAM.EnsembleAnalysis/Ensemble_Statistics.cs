﻿using CartographicCoordinates;
//using DataStructures;
using MathNet.Numerics.Distributions;
using StructOperations;
using System;
using System.Linq;
using System.Net;
using static StructOperations.ArrayManagement;
using static StructOperations.ArrayOperations;


namespace EnsembleAnalysis
{
    public class Ensemble_Statistics
    { 
        public static double[] Extract_ConfidenceLevels1D(double[] H, double[] percentageLevel, double delta = 0.01)
        {
            double meanH = H.Average();
            List<double> tolList = new();


            if (H.Min() == H.Max())
                tolList = Zeros1D(percentageLevel.Length, 0.0).ToList();

            else
            {
                foreach (double conf in percentageLevel)
                {
                    // Reset count
                    double dtol = delta * ArrayAbsolute(ArraySubstract(H, meanH) ).Max();
                    double prc_in = 0;
                    double cont = 0;
                    double tol = 0;

                    //Search tolerance
                    while (prc_in <= conf/100)
                    {
                        cont++;
                        tol = dtol * cont;

                        int[] row = ArrayAbsolute(ArraySubstract(H, meanH))
                            .Select((v, i) => new { v, i })
                            .Where(x => x.v <= tol)
                            .Select(x => x.i)
                            .ToArray();

                        double[] AM = H.Select((v, i) => new { v, i })
                            .Where(x => row.Contains(x.i))
                            .Select(x => x.v)
                            .ToArray();

                        prc_in = (double)AM.Length / (double)H.Length;
                    }
                    tolList.Add(tol);
                }            
            }

            return tolList.ToArray();
        }

        public static double[] Extract_ConfidenceLevels2D(int[,] H, double[] percentageLevel)
        {
            // Calculates de tolerance level for a given percentage of 2D-samples that deviate from the mean.

            int sumH = Sum(H);

            int Delta_l = ArrayFlatten(H).Max() - ArrayFlatten(H).Min();

            double dl = Delta_l / 100.0;
            double cl = ArrayFlatten(H).Min();


            // Empty lists and switches
            int swtch_G = 1;
            int[] swtch_L = Ones1D(percentageLevel.Length, 1);
            double[] levels = Zeros1D(percentageLevel.Length, (double)0);


            while (swtch_G == 1)
            {
                double[,] mtx = ArraySubstract(H, cl);
                double[] mtx_sel_flat = (from mtxValue in ArrayFlatten(mtx) 
                                         select 0.5 * ( 1 + Math.Sign(Math.Sign(mtxValue) + 0.1) )).ToArray();

                double[,] mtx_sel = Reshape2D(mtx_sel_flat, GetSize(mtx));
                double count = Sum( ArrayProduct(mtx_sel, H) ) / sumH;


                // Iterate over array of confidence intervals
                for (int i = 0; i < percentageLevel.Length; i++)
                {
                    // Switch off search for respective level
                    if (swtch_L[i] == 1)
                    {
                        eval_tolerance(count, percentageLevel[i] / 100.0, cl, out double level, out int swtch);
                        levels[i] = level;
                        swtch_L[i] = swtch;
                    }

                    // Switch off all
                    if (levels.All(x => x != 0))
                    {
                        swtch_G = 0;
                    }
                }
                cl += dl;
            }
            return levels;
        }


        private static void eval_tolerance(double count, double tolerance, double cl, out double level, out int swtch)
        {
            // Function to assing level when tolerance is met
            level = 0;
            swtch = 1;

            if (count <= tolerance)
            {
                level = cl; // tolerance level found;
                swtch = 0;  // switch off search;
            }
        }
    }
}
