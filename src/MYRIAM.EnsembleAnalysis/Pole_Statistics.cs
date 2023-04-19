using ContourBuilder;
using DataStructures;
using MYRIAM;
using Histograms;
using System;
using Utilities;


namespace EnsembleAnalysis
{


    class Pole_Statistics
    {
        /// <summary>
        /// This function constructs the contour whithin which the 
        /// a given percentage of the most-recurrent 2D points fall.
        /// </summary>
        /// <remarks>
        /// This function takes 2 arrays of xy coordinates and turns
        /// them into a 2D histogram (MakeHistogram2D) with bins that 
        /// record the spatial recurrence of coordinates. This 
        /// recurrence is now the Z value, whereas the 2D-bin center 
        /// is the X an Y coordinates. The CreateContour method then
        /// finds the level for each given confidence interval
        /// percentage, i.e. the Z-value that encloses where a given
        /// percentage of most-recurrent samples fall and inner
        /// function CreateIsobar returns the coordinates for that 
        /// z-value isobar.
        /// </remarks>
        /// <param name="xArray">Array with the x coordinates.</param>
        /// <param name="yArray">Array with the y coordinates.</param>
        /// <param name="confPercentArray">Confidence interval percentages.</param>
        /// <param name="DM_CNTR_BINS"></param>
        /// <param name="DM_BINS_OUT"></param>
        /// <returns>Array of contours, one structure for each given confidence interval.</returns>
        /// <exception cref="InputErrorException"></exception>
        public static Contour[] Extract_PoleContours(double[] xArray, double[] yArray, double[] confPercentArray,
                                                     double[] DM_CNTR_BINS, out double[] DM_BINS_OUT)
        {

            // =========== Poles Grid Histogram2D ============================================

            // --- Check SPH_PDD_BINS inputs, if given ---

            double gStep = DM_CNTR_BINS[0];
            double xBinMin = DM_CNTR_BINS[1];
            double xBinMax = DM_CNTR_BINS[2];
            double yBinMin = DM_CNTR_BINS[3];
            double yBinMax = DM_CNTR_BINS[4];


            // Check inputs
            if (!double.IsNaN(gStep) && !double.IsNaN(xBinMin))
            {
                // Throw error if given step is larger than given ranges
                if (gStep > xBinMax - xBinMin)
                    throw new InputErrorException(
                        "Error in DM_CNTR_BINS. " +
                        "Given contouring grid resolution exceeds the given longitude range."
                        );

                if (gStep > yBinMax - yBinMin)
                    throw new InputErrorException(
                        "Error in DM_CNTR_BINS. " +
                        "Given contouring grid resolution exceeds the given latitude range.");


                // Throw warning when given ranges are undivisable by the given step
                if ((xBinMax - xBinMin) % gStep != 0)
                    Console.WriteLine(
                        "Warning! " +
                        "Given longitude contouring range is not divisible by the given grid resolution."
                        );

                if ((yBinMax - yBinMin) % gStep != 0)
                    Console.WriteLine(
                        "Warning! " +
                        "Given latitude contouring range is not divisible by the given grid resolution."
                        );
            }



            //  --- Set Historgram2D params ---

            int[]? nBins = null;
            double[][]? binEdges = null;
            double[,]? range = null;


            // Boundaries not given
            if (double.IsNaN(xBinMin))
            {
                // Set grid step if not given (gStep)
                if (double.IsNaN(gStep))
                    gStep = Math.Round((xArray.Max() - xArray.Min()) / 99, 1);


                // Set boundaries that ensure divisibility by gStep
                xBinMin = Math.Floor(xArray.Min() / gStep) * gStep;
                xBinMax = Math.Ceiling(xArray.Max() / gStep) * gStep;
                yBinMin = Math.Floor(yArray.Min() / gStep) * gStep;
                yBinMax = Math.Ceiling(yArray.Max() / gStep) * gStep;


                // Set number of bins (nBins)
                nBins = new int[]
                {
                    (int)Math.Ceiling((xBinMax - xBinMin) / gStep),
                    (int)Math.Ceiling((yBinMax - yBinMin) / gStep),
                };

                range = new double[2, 2] { { xBinMin, xBinMax }, { yBinMin, yBinMax } };
            }


            // Boundaries given
            else
            {
                // Set grid step if not given (gStep)
                if (double.IsNaN(gStep))
                    gStep = (xBinMax - xBinMin) / 100;


                // Set boundaries equally spaced bin edges
                double[] lo = Utils.Arange(xBinMin, xBinMax, gStep).ToArray();
                double[] la = Utils.Arange(yBinMin, yBinMax, gStep).ToArray();


                // Add one more bin if Arange fuction cut highest value out
                if (lo.Last() < xBinMax)
                    lo = lo.Concat(new double[] { lo.Last() + gStep }).ToArray();

                if (la.Last() < yBinMax)
                    la = la.Concat(new double[] { la.Last() + gStep }).ToArray();

                binEdges = new double[2][] { lo, la };
            }



            //  --- Build 2D Histogram ---
            Histogram2D hist2D = new ();

            Parallel.Invoke(
                () => Console_Banners.WriteReports(13, 1, 1),
                () => HistogramBuilder.MakeHistogram2D(out hist2D, xArray, yArray, nBins, binEdges, range)
                );


            // --- Extract Contour Boundaries --
            Contour[] contourArray = hist2D.CreateContour(confPercentArray);


            // --- DM_BINS_OUT parameters used report ---
            DM_BINS_OUT = new double[] { gStep, xBinMin, xBinMax, yBinMin, yBinMax };


            return contourArray;
        }
    }
}
