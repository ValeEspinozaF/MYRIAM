using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Histogram
{
    public class HistogramBin1D
    {
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public double Width { get; set; }

        public HistogramBin1D(double lowerBound, double upperBound, double width)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Width = width;
        }
    }
    public class Histogram1D
    {
        public double Length { get; set; }
        public int[] Values { get; set; }
        public double[] Edges { get; set; }
        public double[] MidBins { get; set; }
        public KeyValuePair<HistogramBin1D, int>[] BinsAndValues { get; set; }

        public Histogram1D(double length, int[] values, double[] bounds, double width)
        {
            Length = length;
            Values = values;
            Edges = bounds;
            MidBins = new double[(int)length];
            BinsAndValues = new KeyValuePair<HistogramBin1D, int>[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                HistogramBin1D binVars = new(bounds[i], bounds[i + 1], width);
                BinsAndValues[i] = new KeyValuePair<HistogramBin1D, int>(binVars, values[i]);

                MidBins[i] = (bounds[i] + bounds[i + 1]) / 2;
            }
        }
        public static Histogram1D MakeHistogram(
                double[] source, int? nBins = null, double binSize = double.NaN,
                double min = double.NaN, double max = double.NaN)
        {
            // --- Check given inputs ---

            if (nBins <= 0)
                throw new ArgumentException("nBins must be positive integer.");

            if (binSize <= 0)
                throw new ArgumentException("BinSize must be positive value.");

            if (nBins.HasValue && !double.IsNaN(binSize))
            {
                if ((double.IsNaN(min) && double.IsNaN(max)) || (!double.IsNaN(min) && !double.IsNaN(max)))
                    throw new ArgumentException("To set nBins and binSize at the same time, must supply either min or max.");
            }

            if (!double.IsNaN(min) && !double.IsNaN(max) && min >= max)
                throw new ArgumentException("Max must be set to a value higher than min");

            if (double.IsNaN(min) && double.IsNaN(max) && source.Min() == source.Max())
                throw new ArgumentException("Source's values are all equal. Can't assign binSize.");



            // --- Set nBins, binSize, min and max values ---

            // If nBins is given
            if (nBins.HasValue)
            {
                // If binSize is given
                if (!double.IsNaN(binSize))
                {
                    if (!double.IsNaN(min))
                        max = (double)(min + nBins * binSize);

                    else if (!double.IsNaN(max))
                        min = (double)(max - nBins * binSize);

                }

                else
                {
                    if (!double.IsNaN(min) && !double.IsNaN(max))
                        nBins--;

                    if (double.IsNaN(min))
                        min = source.Min();

                    if (double.IsNaN(max))
                        max = source.Max();

                    binSize = (double)((max - min) / nBins);

                }
            }

            else
            {
                // If binSize is given
                if (!double.IsNaN(binSize))
                {
                    if (double.IsNaN(min) && double.IsNaN(max))
                    {
                        min = source.Min();
                        max = source.Max();

                        nBins = (int)Math.Ceiling((max - min) / binSize);
                        max = (double)(min + nBins * binSize);
                    }

                    else if (double.IsNaN(min) && !double.IsNaN(max))
                    {
                        min = source.Min();

                        nBins = (int)Math.Ceiling((max - min) / binSize);
                        min = (double)(max - nBins * binSize);
                    }

                    else if (!double.IsNaN(min) && double.IsNaN(max))
                    {
                        max = source.Max();

                        nBins = (int)Math.Ceiling((max - min) / binSize);
                        max = (double)(min + nBins * binSize);
                    }

                    else if (!double.IsNaN(min) && !double.IsNaN(max))
                        throw new ArgumentException("Yet to programm.");

                }

                else
                {
                    nBins = (int)Math.Ceiling(Math.Sqrt(source.Length));

                    if (!double.IsNaN(min) && !double.IsNaN(max))
                        nBins--;

                    if (double.IsNaN(min))
                        min = source.Min();

                    if (double.IsNaN(max))
                        max = source.Max();

                    binSize = (double)((max - min) / nBins);
                }
            }


            // --- Fill bins (binCount) ---

            int[] binCount = new int[(int)nBins];
            double[] bounds = Utils.Linspace(min, max, (int)(nBins + 1));

            for (int i = 0; i < source.Length; i++)
            {
                int binIndex;
                binIndex = (int)((source[i] - min) / binSize);
                if (binIndex == nBins)
                    binIndex--;

                try
                {
                    binCount[binIndex]++;
                }
                catch { }
            }

            // Store bin data as histrogram structure
            return new Histogram1D((int)nBins, binCount, bounds, binSize);
        }
    }
}
