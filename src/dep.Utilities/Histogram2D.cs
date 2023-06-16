using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructOperations;
using DataStructures;
using Cartography;
using Utilities;


namespace Histogram
{
    /// <summary>
    /// Properties and metrics of a bi-dimensional histogram of two data samples.
    /// </summary>
    public class Histogram2D
    {
        public struct HistogramBin2D
        {
            public PositionXY LowerLeftBound { get; set; }
            public PositionXY UpperRightBound { get; set; }
            public PositionXY MidCoordinates { get; set; }
            public double WidthX { get; set; }
            public double WidthY { get; set; }
            public HistogramBin2D(Coordinate llCoord, Coordinate urCoord, Coordinate midCoord, double widthX, double widthY)
            {
                LowerLeftBound = llCoord;
                UpperRightBound = urCoord;
                MidCoordinates = midCoord;
                WidthX = widthX;
                WidthY = widthY;
            }
        }

        public int[] Nbins { get; set; }
        public int[,] Values { get; set; }
        public double[] EdgesX { get; set; }
        public double[] EdgesY { get; set; }
        public double[] WidthX { get; set; }
        public double[] WidthY { get; set; }
        public KeyValuePair<HistogramBin2D, int>[,] BinsAndValues { get; set; }


        /// <summary>
        /// Compute the bi-dimensional histogram of two data arrays, containg 
        /// each the Lon and Lat coordinates of a sample.
        /// </summary>
        /// <remarks>
        /// If <paramref name="nBins"/> nor <paramref name="binEdges"/> paramaters 
        /// are supplied, the grid will be divided in 50 bins on each dimension. 
        /// Also, if both <paramref name="binEdges"/> and <paramref name="range"/>
        /// parameters are given, <paramref name="range"/> is ignored.
        /// </remarks>
        /// <typeparam name="T">Type of array; will be processed as double.</typeparam>
        /// <param name="arrayX">Array of x-coordinates</param>
        /// <param name="arrayY">Array of y-coordinates</param>
        /// <param name="nBins">Number of bins for each (Length == 2) or both dimensions (Length == 1). Optional.</param>
        /// <param name="binEdges">Bin edges for each (Length == 2) or both dimensions (Length == 1). Optional.</param>
        /// <param name="range">Minimum and maximun bin edges for each dimension (Size = 2,2). Optional. </param>
        /// <returns>Histogram2D instance.</returns>
        /// <exception cref="ArgumentException">Thrown when:
        /// <para>Arrays <paramref name="arrayX"/> and <paramref name="arrayY"/> do not have the same length.</para>
        /// <para>Inputs <paramref name="nBins"/> and <paramref name="binEdges"/> are both supplied.</para>
        /// <para>Input <paramref name="nBins"/> is empty or has more values than 2 values (one for each dimension).</para>
        /// <para>Input <paramref name="nBins"/> is not a positive integer bigger than zero.</para>
        /// <para>Input <paramref name="binEdges"/> values must be in increasing order.</para>
        /// <para>Input <paramref name="range"/> values must be increasing in each dimension.</para>
        /// </exception>
        public Histogram2D(double[] arrayX, double[] arrayY, int[]? nBins = null, double[][]? binEdges = null, double[,]? range = null)
        {
            /*
             * The following code and its dependencies (get_outerEdges, searchSorted, ravel_multiIndex, binCount) 
             * are modified from numpy source code (Under the BSD 3-Clause).
             * License and copyright notice under folder "Licenses".
             */

            // --- Check given inputs ---

            if (arrayX.Length != arrayY.Length)
                throw new ArgumentException("Lon and Lat arrays must have the same length.");

            if (nBins != null && binEdges != null)
                throw new ArgumentException("Cannot supply both nBins and BinEdges.");

            if (nBins != null && nBins.Length != 1 && nBins.Length != 2)
                throw new ArgumentException("nBins must have either one or two bin count.");

            if (nBins != null && nBins.Any(x => x <= 0))
                throw new ArgumentException("nBins must be contain positive integers.");

            if (binEdges != null && range != null)
                Console.WriteLine("\nWarning! When both binEdges and range are supplied, range is ignored.\n");



            // --- Correct shape of inputs, if needed ---

            // If nBins is given as a single value array, duplicate it to the needed 1x2 dimensions
            if (nBins != null && nBins.Length == 1)
                nBins = ArrayManagement.Repeat1D(nBins[0], 2);


            // If binEdges is given as a single array array, duplicate it to the needed 2xn dimensions
            if (binEdges != null && binEdges.Length == 1)
                binEdges = ArrayManagement.Repeat2D(binEdges[0], 2);


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
                        rangei = genericToDouble(ArrayManagement.GetRow(range, i));

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
            int[] binCount = countBins(xy, minlength: ArrayOperations.Product(nbin));


            // Reshape into a 2D matrix
            int[,] binCount2D = ArrayManagement.Reshape2D(binCount, nbin);


            // Remove outlier bins (indices 0 and -1 for each dimension).
            binCount2D = ArrayManagement.SubArray2D(binCount2D, 1, dedges[1].Length, 1, dedges[0].Length);



            // --- Set output bin spatial distribution as cartographic coordinates ---

            // Transpose and invert so that grid binCount[0,0] is top left map corner
            binCount2D = Matrix.Transpose(binCount2D);
            binCount2D = ArrayManagement.Flip(binCount2D, 0);


            // edges[0] contains y-axis binCount, edges[1] contains x-axis ones 
            edges = ArrayManagement.Flip(edges, 0);


            // edges [0] binCount in decreasing order (from north to south degrees)
            edges[0] = ArrayManagement.Flip(edges[0]);


            // dedges[0] contains y-axis binCount, dedges[1] contains x-axis ones 
            dedges = ArrayManagement.Flip(dedges, 0);


            // Fill Histogram2D properties
            Nbins = nBins;
            Values = binCount2D;
            EdgesX = edges[1];
            EdgesY = edges[0];
            WidthX = dedges[1];
            WidthY = dedges[0];
            BinsAndValues = new KeyValuePair<HistogramBin2D, int>[binCount2D.GetLength(0), binCount2D.GetLength(1)];

            for (int i = 0; i < binCount2D.GetLength(0); i++)
            {
                for (int j = 0; j < binCount2D.GetLength(1); j++)
                {
                    HistogramBin2D binVars = new()
                    {
                        LowerLeftBound = new PositionXY(edges[1][j], edges[0][i]),
                        UpperRightBound = new PositionXY(edges[1][j + 1], edges[0][i + 1]),
                        MidCoordinates = new PositionXY((edges[1][j] + edges[1][j + 1]) / 2, (edges[0][i] + edges[0][i + 1]) / 2),
                        WidthX = WidthX[j],
                        WidthY = WidthY[i]
                    };

                    BinsAndValues[i, j] = new KeyValuePair<HistogramBin2D, int>(binVars, binCount2D[i, j]);
                }
            }
            
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


        private static int[] countBins(int[] xArray, int minlength)
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
