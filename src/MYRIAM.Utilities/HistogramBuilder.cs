using static StructOperations.ArrayManagement;
using StructOperations;
using DataStructures;
using Utilities;
using EnsembleAnalysis;
using ContourBuilder;

namespace Histograms
{
    public struct Histogram
    {
        public double Length;
        public int[] Values;
        public double[] Edges;
        public double[] MidBins;
        public KeyValuePair<HistogramBin, int>[] BinsAndValues;
        public Histogram(double length, int[] values, double[] bounds, double width)
        {
            Length = length;
            Values = values;
            Edges = bounds;
            MidBins = new double[(int)length];
            BinsAndValues = new KeyValuePair<HistogramBin, int>[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                HistogramBin binVars = new(bounds[i], bounds[i+1], width);
                BinsAndValues[i] = new KeyValuePair<HistogramBin, int>(binVars, values[i]);

                MidBins[i] = (bounds[i] + bounds[i + 1]) / 2;
            }
        }
    }

    public struct HistogramBin
    {
        public double LowerBound;
        public double UpperBound;
        public double Width;
        public HistogramBin(double lowerBound, double upperBound, double width)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Width = width;
        }
    }


    public struct Histogram2D
    {
        public int[] Nbins;
        public int[,] Values;
        public double[] EdgesX;
        public double[] EdgesY;
        public double[] WidthX;
        public double[] WidthY;
        public KeyValuePair<HistogramBin2D, int>[,] BinsAndValues;
        public Histogram2D(int[] nbins, int[,] values, double[][] edges, double[][] width)
        {
            Nbins = nbins;
            Values = values;
            EdgesX = edges[1];
            EdgesY = edges[0];
            WidthX = width[1];
            WidthY = width[0];
            BinsAndValues = new KeyValuePair<HistogramBin2D, int>[values.GetLength(0), values.GetLength(1)];

            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    var lowerLeftBound = new Coord( edges[1][j], edges[0][i] );
                    var upperRightBound = new Coord( edges[1][j + 1], edges[0][i + 1] );
                    var midCoordinates = new Coord( (edges[1][j] + edges[1][j + 1])/2, (edges[0][i] + edges[0][i + 1])/2);

                    HistogramBin2D binVars = new(lowerLeftBound, upperRightBound, midCoordinates, WidthX[j], WidthY[i]);
                    BinsAndValues[i, j] = new KeyValuePair<HistogramBin2D, int>(binVars, values[i, j]);
                }
            }
        }

        public void CreateContour(double[] confPercent, out Contour[] contour)
        {
            contour = CreateContour(confPercent);
        }

        public Contour[] CreateContour(double[] confPercent)
        {
            Contour[] contours = new Contour[confPercent.Length];

            // Determine level for each percentage tolerance
            double[] levels = Ensemble_Statistics.Extract_ConfidenceLevels2D(Values, confPercent);

            for (int i = 0; i < confPercent.Length; i++)
            {
                // Find dataset's level for given confidence invertal (in percentage)
                var isoBars = Isobar.CreateIsobar(this, levels[i]);

                // Store data as Contour structure
                contours[i] = new Contour(isoBars, levels[i], confPercent[i]);
            }

            return contours;
        }
    }


    public struct HistogramBin2D
    {
        public Coord LowerLeftBound;
        public Coord UpperRightBound;
        public Coord MidCoordinates;
        public double WidthX;
        public double WidthY;
        public HistogramBin2D(Coord lowerLeftBound, Coord upperRightBound, Coord midCoordinates, double widthX, double widthY)
        {
            LowerLeftBound = lowerLeftBound;
            UpperRightBound = upperRightBound;
            MidCoordinates = midCoordinates;
            WidthX = widthX;
            WidthY = widthY;
        }
    }


    class HistogramBuilder
    {
        public static Histogram MakeHistogram(double[] source, int? nBins = null, double binSize = double.NaN, double min = double.NaN, double max = double.NaN)
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
                catch {}
            }

            // Store bin data as histrogram structure
            return new Histogram ((int)nBins, binCount, bounds, binSize);
        }

        public static void MakeHistogram2D<T>(out Histogram2D hist2D, T[] arrayX, T[] arrayY, int[]? nBins = null, T[][]? binEdges = null, T[,]? range = null)
        {
            hist2D = MakeHistogram2D<T>(arrayX, arrayY, nBins, binEdges, range);
        }


        /// <summary>
        /// Compute the bi-dimensional histogram of two data arrays, containg 
        /// each the X and Y coordinates of a sample.
        /// </summary>
        /// <remarks>
        /// If <paramref name="nBins"/> nor <paramref name="binEdges"/> paramaters 
        /// are supplied, the grid will be divided in 50 bins on each dimension. 
        /// Also, if both <paramref name="binEdges"/> and <paramref name="range"/>
        /// parameters are given, <paramref name="range"/> is ignored.
        /// </remarks>
        /// <typeparam name="T">Type of array; will be processed as double.</typeparam>
        /// <param name="arrayX">Array of X-coordinates</param>
        /// <param name="arrayY">Array of Y-coordinates</param>
        /// <param name="nBins">Number of bins for each (Length == 2) or both dimensions (Length == 1). Optional</param>
        /// <param name="binEdges">Bin edges for each (Length == 2) or both dimensions (Length == 1). Optional</param>
        /// <param name="range">Min and maximun limits for each dimension (Size = 2,2). Optional </param>
        /// <returns>Histogram2D structure with the resulting bin values</returns>
        /// <exception cref="ArgumentException">Thrown when:
        /// <para>Arrays <paramref name="arrayX"/> and <paramref name="arrayY"/> do not have the same length.</para>
        /// <para>Inputs <paramref name="nBins"/> and <paramref name="binEdges"/> are both supplied.</para>
        /// <para>Input <paramref name="nBins"/> is empty or has more values than 2 values (one for each dimension).</para>
        /// <para>Input <paramref name="nBins"/> must be a positive integer bigger than zero.</para>
        /// <para>Input <paramref name="binEdges"/> values must be in increasing order.</para>
        /// <para>Input <paramref name="range"/> values must be increasing in each dimension.</para>
        /// </exception>
        public static Histogram2D MakeHistogram2D<T>(T[] arrayX, T[] arrayY, int[]? nBins = null, T[][]? binEdges = null,  T[,]? range = null)
        {
            /*
             * The following code and its dependencies (get_outerEdges, searchSorted, ravel_multiIndex, binCount) 
             * are modified from numpy source code (Under the BSD 3-Clause).
             * License and copyright notice under folder "Licenses".
             */

            // --- Check given inputs ---

            if (arrayX.Length != arrayY.Length)
                throw new ArgumentException("X and Y arrays must have the same length.");

            if (nBins != null && binEdges != null)
                throw new ArgumentException("Cannot supply both nBins and BinEdges.");

            if (nBins != null && nBins.Length != 1 && nBins.Length != 2)
                throw new ArgumentException("nBins must have either one or two bin count.");

            if (nBins != null && nBins.Any(x => x <= 0))
                throw new ArgumentException("nBins must be contain positive integers.");

            if (binEdges != null && range != null)
                Console.WriteLine("Warning! When both binEdges and range are supplied, range is ignored.");



            // --- Correct shape of inputs, if needed ---

            // If nBins is given as a single value array, duplicate it to the needed 1x2 dimensions
            if (nBins != null && nBins.Length == 1)
                nBins = Repeat1D(nBins[0], 2);


            // If binEdges is given as a single array array, duplicate it to the needed 2xn dimensions
            if (binEdges != null && binEdges.Length == 1)
                binEdges = Repeat2D(binEdges[0], 2);


            // If nBins nor binEdges are given, set default nBins to a 10x10 grid
            if (nBins == null && binEdges == null)
                nBins = new int[] { 50, 50 };



            // --- Set histogram empty output arrays --- 

            double[][] edges = new double[2][];
            double[][] dedges = new double[2][];
            int[] nbin = new int[2];



            // --- Set sample as 2D array --- 

            double[][] sample = new double[2][];
            sample[0] = genericToDouble(arrayX);
            sample[1] = genericToDouble(arrayY);



            // --- Create edge arrays ---

            for (int i = 0; i < 2; i++)
            {
                // binEdges are given
                if (binEdges != null)
                {
                    // Check that binEdges array binCount are in ascending order
                    if (!Utils.CheckIncreasing(genericToDouble(binEdges[i])))
                        throw new ArgumentException("BinEdges must be increasing.");

                    edges[i] = genericToDouble(binEdges[i]);
                }

                else
                {
                    double[] samplei = sample[i];
                    double[]? rangei = null;

                    if (range != null)
                        rangei = genericToDouble(GetRow(range, i));

                    // Get outer edges from either sample array or range if given
                    double[] sMinMax = get_outerEdges(samplei, rangei);
                    double sMin = sMinMax[0];
                    double sMax = sMinMax[1];

                    int n = nBins[i];

                    // Include an outlier-bin on each end
                    edges[i] = Utils.Linspace(sMin, sMax, n + 1);
                }


                // Store nbin and edges binCount
                nbin[i] = edges[i].Length + 1;
                dedges[i] = Utils.Difference(edges[i]);
            }


            // --- Fill bins (binCount) ---

            // Compute the bin number each sample falls into
            int[][] Ncount = new int[2][];
            for (int j = 0; j < 2; j++)
                Ncount[j] = searchSorted(edges[j], sample[j], "right");


            // Values equal to the right edge (in the rightmost outlier-bin),
            // ougth to be counted in the rightmost non-outlier bin
            for (int h = 0; h < 2; h++)
            {
                for (int g = 0; g < sample[h].Length; g++)
                    if (sample[h][g] == edges[h].Last())
                        Ncount[h][g]--;
            }

            // Compute the sample indices in a flattened histogram matrix
            int[] xy = ravel_multiIndex(Ncount, nbin);


            // Compute the number of repetitions in xy and assign it to the flattened histmat
            int[] binCount = HistogramBuilder.binCount(xy, minlength: ArrayOperations.Product(nbin));


            // Reshape into a 2D matrix
            int[,] binCount2D = Reshape2D(binCount, nbin);


            // Remove outlier bins (indices 0 and -1 for each dimension).
            binCount2D = SubArray2D(binCount2D, 1, dedges[1].Length, 1, dedges[0].Length);



            // --- Set output bin spatial distribution as cartographic coordinates ---

            // Transpose and invert so that grid binCount[0,0] is top left map corner
            binCount2D = MatrixOperations.MatrixTranspose(binCount2D);
            binCount2D = Flip(binCount2D, 0);


            // edges[0] contains y-axis binCount, edges[1] contains x-axis ones 
            edges = Flip(edges, 0);


            // edges [0] binCount in decreasing order (from north to south degrees)
            edges[0] = Flip(edges[0]);


            // dedges[0] contains y-axis binCount, dedges[1] contains x-axis ones 
            dedges = Flip(dedges, 0);


            // Store bin data as histrogram structure
            return new Histogram2D(nBins, binCount2D, edges, dedges);
        }


        private static double[] genericToDouble<T>(T[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = Convert.ToDouble(array[i]);
            }
            return result;
        }


        private static double[] get_outerEdges(double[] a, double[]? range = null)
        {
            // Determine the outer bin edges to use, from either the data or the range argument

            double first_edge;
            double last_edge;

            if (range != null)
            {
                first_edge = range[0];
                last_edge = range[1];

                if (first_edge > last_edge)
                    throw new ArgumentException("Max must be larger than min in range parameter.");
            }

            // Handle empty array
            else if (a.Length == 0)
            {
                first_edge = 0;
                last_edge = 1;
            }

            else
            {
                first_edge = a.Min();
                last_edge = a.Max();
            }

            // Expand empty range to avoid divide by zero
            if (first_edge == last_edge)
            {
                first_edge -= 0.5;
                last_edge += 0.5;
            }
            return new double[2] { first_edge, last_edge };
        }


        private static int[] searchSorted(double[] a, double[] v, string side = "left")
        {
            // Find indices where elements should be inserted to maintain order.

            if (side != "left" && side != "right")
                throw new ArgumentException("Side argument must be either 'left' or 'right'.");

            int[] sortedIndexArray = new int[v.Length];

            for (int n = 0; n < v.Length; n++)
            {
                if (side == "left")
                {
                    if (v[n] <= a[0])
                        sortedIndexArray[n] = 0;

                    else if (a.Last() < v[n])
                        sortedIndexArray[n] = a.Length;
                }
                else if (side == "right")
                {
                    if (v[n] < a[0])
                        sortedIndexArray[n] = 0;

                    else if (a.Last() <= v[n])
                        sortedIndexArray[n] = a.Length;
                }


                for (int i = 1; i < a.Length; i++)
                {
                    if (side == "left")
                    {
                        if (a[i - 1] < v[n] && v[n] <= a[i])
                            sortedIndexArray[n] = i;
                    }
                    else if (side == "right")
                    {
                        if (a[i - 1] <= v[n] && v[n] < a[i])
                            sortedIndexArray[n] = i;
                    }
                }
            }
            return sortedIndexArray;
        }


        private static int[] ravel_multiIndex(int[][] multi_index, int[] dims)
        {
            // Converts a tuple of index arrays into an array of flat indices, 
            // applying boundary modes to the multi-index

            if (dims.Length != 2)
                throw new ArgumentException("Functionality set for only two dimensions.");

            int[] multiIndex = (from i in Enumerable.Range(0, multi_index[0].Length)
                                select multi_index[0][i] * dims[1] + multi_index[1][i]).ToArray();

            return multiIndex;
        }


        private static int[] binCount(int[] xArray, int minlength)
        {
            // Count number of occurrences of each value in array of non-negative ints

            int[] values = new int[minlength];

            for (int i = 0; i < xArray.Length; i++)
            {
                int binIndex = xArray[i];

                if (binIndex == minlength)
                    binIndex--;
                
                values[binIndex]++;
            }

            return values;
        }
    }
}
