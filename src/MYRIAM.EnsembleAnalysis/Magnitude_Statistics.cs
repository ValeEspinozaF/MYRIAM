using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms;



namespace EnsembleAnalysis
{
    class Magnitude_Statistics
    {
        public static double[,] Extract_MagnitudeBinContour(double[] magArray, out Histogram hist,
                                                            int? nBins = null, double binSize = double.NaN, 
                                                            double min = double.NaN, double max = double.NaN)
        {
            // --- Histogram ---

            // Create histogram 
            hist = HistogramBuilder.MakeHistogram(magArray, nBins, binSize, min, max);



            // Store recurrence in each bin (histValues) and edges of bins (histEdges)
            int[] histValues = hist.Values;
            double[] histEdges = hist.Edges;



            // --- Bins XY vcurve ---

            // Create line of bins outline
            double[,] magHist_XY = new double[histEdges.Length * 2, 2];
            for (int i = 1; i < histValues.Length * 2; i += 2)
            {
                magHist_XY[i, 0] = histEdges[i / 2];
                magHist_XY[i, 1] = histValues[i / 2];

                magHist_XY[i + 1, 0] = histEdges[i / 2 + 1];
                magHist_XY[i + 1, 1] = histValues[i / 2];
            }

            // Fill edges
            magHist_XY[0, 0] = histEdges[0];
            magHist_XY[0, 1] = 0.0;
            magHist_XY[histEdges.Length * 2 - 1, 0] = histEdges[^1];
            magHist_XY[histEdges.Length * 2 - 1, 1] = 0.0;

            return magHist_XY;
        }
    }
}
