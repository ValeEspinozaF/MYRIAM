using System;
using System.Text;
using Utilities;
using ContourBuilder;
using Histogram;


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
        /// recurrence is now the Val value, whereas the 2D-bin center 
        /// is the Lon an Lat coordinates. The CreateContour method then
        /// finds the level for each given confidence interval
        /// percentage, i.e. the Val-value that encloses where a given
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
        public static Contour[] Extract_PoleContours(
            double[] xArray, double[] yArray, double[] confPercentArray, out double[] outParams,
            double[]? xBinRange = null, double[]? yBinRange = null, double gStep = double.NaN)
        {

            // =========== Poles Grid Histogram2D ============================================


            set_HistogramParams(
                xArray, yArray, xBinRange, yBinRange, gStep,
                out outParams, out int[] nBins, out double[][] binEdges, out double[,] range);


            //  --- Build 2D Histogram1D ---
            Histogram2D hist2D = Histogram2D.MakeHistogram2D(xArray, yArray, nBins, binEdges, range);


            // --- Extract Contour Boundaries --
            Contour[] contourArray = Contour.CreateContour(hist2D, confPercentArray);


            return contourArray;
        }

        private static void set_HistogramParams(
            double[] xArray, double[] yArray,
            double[] xBinRange, double[] yBinRange, double gStep,
            out double[] outParams, out int[] nBins, out double[][] binEdges, out double[,] range)
        {
            //  --- Check inputs ---

            double xBinMin = double.NaN;
            double xBinMax = double.NaN;
            double yBinMin = double.NaN;
            double yBinMax = double.NaN;

            if (xArray != null)
            {
                xBinMin = xBinRange[0];
                xBinMax = xBinRange[1];
            }

            if (yArray != null)
            {
                yBinMin = yBinRange[0];
                yBinMax = yBinRange[1];
            }


            if (!double.IsNaN(gStep) && !double.IsNaN(xBinMin))
            {
                // Throw error if given step is larger than given ranges
                if (gStep > xBinMax - xBinMin)
                    throw new ArgumentException(
                        "Given contouring grid resolution (gStep) exceeds the given longitude range (xBinRange)."
                        );

                // Throw warning when given ranges are undivisable by the given step
                if ((xBinMax - xBinMin) % gStep != 0)
                    Console.WriteLine(
                        "Warning! " +
                        "Given longitude contouring range is not divisible by the given grid resolution."
                        );
            }

            if (!double.IsNaN(gStep) && !double.IsNaN(xBinMin))
            {
                if (gStep > yBinMax - yBinMin)
                    throw new ArgumentException(
                        "Given contouring grid resolution exceeds the given latitude range.");

                if ((yBinMax - yBinMin) % gStep != 0)
                    Console.WriteLine(
                        "Warning! " +
                        "Given latitude contouring range is not divisible by the given grid resolution."
                        );
            }



            //  --- Set Historgram2D params ---

            nBins = null;
            binEdges = null;
            range = null;


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
                double[] lo = Utils.Arange(xBinMin, xBinMax, gStep);
                double[] la = Utils.Arange(yBinMin, yBinMax, gStep);


                // Add one more bin if Arange fuction cut highest value out
                if (lo.Last() < xBinMax)
                    lo = lo.Concat(new double[] { lo.Last() + gStep }).ToArray();

                if (la.Last() < yBinMax)
                    la = la.Concat(new double[] { la.Last() + gStep }).ToArray();

                binEdges = new double[2][] { lo, la };
            }

            outParams = new double[] { gStep, xBinMin, xBinMax, yBinMin, yBinMax };
        }
    }
}
