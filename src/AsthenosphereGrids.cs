using Cartography;
using StructOperations;
using System.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MYRIAM.FileReader;
using static StructOperations.ArrayOperations;
using static StructOperations.ArrayManagement;
using Utilities;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Reflection;
using MYRIAM.Properties;

namespace MYRIAM
{
    /// <summary>
    /// Uploads, calculates and stores all the necessary grids (e.g., temperature, viscosity, Young's Modulus) used in the depth-average calculations within the asthenosphere. 
    /// Base data and methods taken from Priestley & McKenzie (2013), Paulson & Richards(2009) and Dziewonski & Anderson (1981).
    /// </summary>
    public class AsthenosphereGrids
    {
        /// <summary>
        /// Average thickness of the asthenosphere channel in meters.
        /// </summary>
        public double HA { get; set; }

        /// <summary>
        /// 2D grid with the longitude (in degrees) for each datapoint.
        /// </summary>
        public double[,] muA_mlon { get; set; }

        /// <summary>
        /// 2D grid with the latitude (in degrees) for each datapoint.
        /// </summary>
        public double[,] muA_mlat { get; set; }

        /// <summary>
        /// 2D grid with the asthenosphere viscosity (in Pa · s) for each datapoint.
        /// </summary>
        public double[,] muA_mdat { get; set; }

        /// <summary>
        /// 2D grid with the asthenosphere Young's modulus (in Pa) for each datapoint.
        /// </summary>
        public double[,] YM_mdat { get; set; }

        /// <summary>
        /// 2D grid with the Maxwell time period expressed in years for each datapoint.
        /// </summary>
        public double[,] MT_mdat { get; set; }


        public struct GridLayer_PM13_T
        {
            /// <summary>
            /// Longitude in degrees-East.
            /// </summary>
            public double[,] LonGrid { get; set; }

            /// <summary>
            /// Latitude in degree-North.
            /// </summary>
            public double[,] LatGrid { get; set; }

            /// <summary>
            /// Temperature in degrees-Celsius.
            /// </summary>
            public double[,] TempGrid { get; set; }

            /// <summary>
            /// Number of grid rows.
            /// </summary>
            public int gRows { get; set; }

            /// <summary>
            /// Number of grid columns.
            /// </summary>
            public int gCols { get; set; }

            public static GridLayer_PM13_T Upload_Grid(double depth)
            {
                // Set depth string
                string DPT;
                if (depth < 100e3)
                    DPT = $"0{depth / 1e3}";
                else
                    DPT = $"{depth / 1e3}";


                // Read PM13 Temperature-model file as lines
                ResourceManager resourceManager = new ResourceManager("MYRIAM.Properties.PriestlyMcKenzey2013_T_model", typeof(PriestlyMcKenzey2013_T_model).Assembly);
                string s = resourceManager.GetString("GRD_PM13_DEPTH" + DPT + "km_T");
                string[] lines = s.Split("\r\n");



                // Set empty 2D arrays
                double[,] lonGrid = new double[90, 180];
                double[,] latGrid = new double[90, 180];
                double[,] tempGrid = new double[90, 180];


                // Fill arrays with parsed values
                for (int i = 0; i < lines.Length; i++)
                {
                    int k = i / 180;
                    var values = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < values.Length; j++)
                    {
                        int column = i - 180 * k;

                        if (j == 0) lonGrid[k, column] = FileReader.GetValue(values[j]);
                        if (j == 1) latGrid[k, column] = FileReader.GetValue(values[j]);
                        if (j == 2) tempGrid[k, column] = FileReader.GetValue(values[j]);
                    }
                }

                return new GridLayer_PM13_T
                {
                    LonGrid = lonGrid,
                    LatGrid = latGrid,
                    TempGrid = tempGrid,
                    gRows = lonGrid.GetLength(0),
                    gCols = lonGrid.GetLength(1),
                };
            }

            public static GridLayer_PM13_T Upload_Grid(double depth, CartoLimits cartoLimits)
            {
                var grid = Upload_Grid(depth);
                return Crop_Grid(grid, cartoLimits);
            }

            public static GridLayer_PM13_T Crop_Grid(GridLayer_PM13_T grid, CartoLimits cartoLimits)
            {
                // --- lonGrid filters ---
                double[] grdCols = ArrayManagement.GetRow(grid.LonGrid, 0);
                var grdCols_pair = grdCols
                    .Select((element, index) => new KeyValuePair<double, double>(index, element))
                    .Where(p => p.Value >= cartoLimits.LonMin && p.Value <= cartoLimits.LonMax)
                    .ToArray();


                var grdCols_keys = (from value in grdCols_pair select (int)value.Key).ToArray();


                // --- LatGrid filters ---
                double[] grdRows = ArrayManagement.GetColumn(grid.LatGrid, 0);
                var grdRows_pair = grdRows
                    .Select((element, index) => new KeyValuePair<double, double>(index, element))
                    .Where(p => p.Value >= cartoLimits.LatMin && p.Value <= cartoLimits.LatMax)
                    .ToArray();

                var grdRows_keys = (from value in grdRows_pair select (int)value.Key).ToArray();


                // --- Crop_Grid arrays ---
                double[,] lonGrid = new double[grdRows_keys.Length, grdCols_keys.Length];
                double[,] LatGrid = new double[grdRows_keys.Length, grdCols_keys.Length];
                double[,] tempGrid = new double[grdRows_keys.Length, grdCols_keys.Length];

                for (int ui = 0; ui < grdRows_keys.Length; ui++)
                {
                    for (int uj = 0; uj < grdCols_keys.Length; uj++)
                    {
                        lonGrid[ui, uj] = grid.LonGrid[grdRows_keys[ui], grdCols_keys[uj]];
                        LatGrid[ui, uj] = grid.LatGrid[grdRows_keys[ui], grdCols_keys[uj]];
                        tempGrid[ui, uj] = grid.TempGrid[grdRows_keys[ui], grdCols_keys[uj]];
                    }
                }

                return new GridLayer_PM13_T
                {
                    LonGrid = lonGrid,
                    LatGrid = LatGrid,
                    TempGrid = tempGrid,
                    gRows = lonGrid.GetLength(0),
                    gCols = lonGrid.GetLength(1),
            };
            }

        }


        /// <summary>
        /// Calculate the average thickness of the asthenosphere channel and grids for the asthenosphere's depth-average viscosity and Young's modulus.
        /// </summary>
        /// <param name="inputParams"><see cref="InputParameters"/> instance with the necessary running parameters (See function overload).</param>
        public void Map_muA_AverageGrid(InputParameters inputParams)
        {
            // ============== Parameters ===============================================

            double muM = inputParams.muM;
            double muA = inputParams.muA;
            double HL = inputParams.HL_km * 1e3;
            double fHA = inputParams.FRACTION_HA;
            CartoLimits REGION_muA_LV = inputParams.REGION_muA_LV;

            Map_muA_AverageGrid(muM, muA, HL, fHA, REGION_muA_LV);
        }


        /// <summary>
        /// Calculate the average thickness of the asthenosphere channel and grids for the asthenosphere's depth-average viscosity and Young's modulus.
        /// </summary>
        /// <param name="muM">Average value of the lower part of the upper mantle viscosity, expressed in Pa · s.</param>
        /// <param name="muA">Average value of the asthenosphere viscosity, expressed in Pa · s.</param>
        /// <param name="HL">Value of the lithosphere thickness (i.e., depth of the lithosphere-asthenosphere boundary), expressed in meters.</param>
        /// <param name="fHA">Value between 0 and 1 that sets the vertical fraction of the region utilized to define the depth-average viscosity of the 
        /// asthenosphere using the model PM_v2_2012 by Priestley and McKenzie, 2013.</param>
        /// <param name="REGION_muA_LV"><see cref="CartoLimits"/> instance with the geographical limits (in degrees) within which <c>muA</c> is average.</param>
        /// 
        /// <remarks>
        /// Function extended explanation:
        /// 
        /// <para>Starting from an input value of the desired asthenosphere (<paramref name="muA"/> [Pa*s]) and upper mantle (<paramref name="muM"/> [Pa * s]) 
        /// average-viscosities, this function calculates the expected thickness of the asthenosphere following Paulson & Richards(2009). Altogether with (i) 
        /// an input value for the maximum thickness of the lithosphere (<paramref name="HL"/> [m]), (ii) asthenosphere's temperature extracted from the 
        /// model of Priestley & McKenzie (2013), and (iii) pressures values at various depth calculated from the PREM model, this script 
        /// also calculates a power-law coefficient for the viscosity(c) that would yield a layer-average equal to muA in input.
        /// This is done building on equations 15-17 of Priestley & McKenzie(2013). The viscosity at any point p is calculated as 
        /// muA(p) = c * exp( (E+P(p) Va) / (RT(p)) ). Once the layer viscosity has been calculated for all model layers that fall within 
        /// the asthenosphere, an output depth average at all positions is calculated (<paramref name="HA"/> [m]), as well as 2D arrays storing the grid 
        /// data points values for longitude (<see cref="muA_mlon"/> [degrees]), latitude (<see cref="muA_mlat"/> [degrees]), and asthenosphere viscosity 
        /// (<see cref="muA_mdat"/> [Pa * s]). Output grid values for Young's modulus (<see cref="YM_mdat"/>) are also obtained using the relationship in 
        /// Priestley & McKenzie(2013), but accounting only for temperature lateral variations - since the pressure at any depth is uniform and calculated 
        /// from PREM.</para>
        /// 
        /// <para>All the above is performed within the geographical rectangular-region specified through the parameter <paramref name="REGION_muA_LV"/>.
        /// Furthermore, one may elect to only use the upper portion of the asthenosphere specified through the parameter <paramref name="fHA"/> 
        /// (between 0 and 1) in order to perform the depth averaging at any position.</para>
        /// </remarks>
        public void Map_muA_AverageGrid(
            double muM,
            double muA,
            double HL,
            double fHA,
            CartoLimits REGION_muA_LV
            )
        {

            // ============== Parameters ===============================================

            // PM13 Resolution (in radians)
            var dlon = 2 * (Math.PI / 180);
            var dlat = 2 * (Math.PI / 180);

            var Re = 6371e3;
            var g = 9.8;

            int nSamples_PR09 = (int)1e4;



            // Upload PREM complete mantle model
            Upload_PremModel(out Dictionary<string, double[]> PREM);


            // Depth [meters] 
            double[] PREM_z = (from radius in PREM["Radius"] select Re - (radius * 1e3)).ToArray();


            // Parameters (See Priestley & McKenzie, EPSL 2013 - Table 1)
            double E = 402.9e3;     // activation energy [J/mol]
            double Va = 7.81e-6;    // activation volume [m^3/mol]
            double R = 8.314;       // gas constant
            double CK_offset = 273.15; // Celsius/Kelvin offset

            // Young's modulus [in Pa], based on Mantle from Turcotte and Schubert
            double Young_Mod_mean = 1.5e11;

            // Derivative of Young modulus with temperature as in Priestley & McKenzie, EPSL 2013 - Table 1. [Pa/K]
            double dYMdT = -0.87 * 1e-2 * 1e9; 

            // Maxwell time (in years)
            double TAU_MXWyr = (muA / Young_Mod_mean) / (365 * 24 * 60 * 60);



            // === Calculate thickness of asthenosphere according to PR09 === 

            // Sample probability constant (A) for D = A * (muA/muM)^(1/3)
            double[] A_samples = PR09_F3_DATA_FIT(nSamples_PR09);


            // Calculate asthenosphere thickness from muA and muM (Paulson & Richards, 2009)
            double[] HA_SAMPLES = ArrayMultiply(A_samples, Math.Pow(muA / muM, 1.0 / 3.0));
            double HA_PR09 = HA_SAMPLES.Average();
            this.HA = HA_PR09;



            // === Layer viscosity on the basis of temperature lateral variations ===

            // Filter layers to depths of asthenosphere (HL_km <= p <= (HL_km + HA_PR09 * FRACTION_HA)) 

            double[] z_PM13_1e3 = Utils.Arange(50e3, 400e3, 25e3); // Available PM13 depths (every 25km)
            double[] z_PM13_2use = z_PM13_1e3.Where(p => p >= HL && p <= (HL + HA_PR09 * fHA)).ToArray();


            // Throw warnings and errors
            if (HL + HA > 400e3)
                Console.WriteLine(
                    "\nWarning! " +
                    "Input HL_km and calculated HA yield an added thickness larger than 400 km (max. depth of PM13).\n");

            if (z_PM13_2use.Length == 0)
                throw new ArgumentException(
                    "Error! " +
                    "Combination of given HL_km and FRACTION_HA yield a layer of thickness smaller " +
                    "than the resolution of model PM_v2_2012 (from 50 to 400 km every 25 km).");


            Console_Banners.WriteReports(4);
            switch (REGION_muA_LV.IsEmpty())
            {
                // No lateral variation
                case true:
                    {
                        // Get data grid
                        GridLayer_PM13_T grid = GridLayer_PM13_T.Upload_Grid(z_PM13_2use[0]);

                        int gRows = grid.gRows;
                        int gCols = grid.gCols;


                        // Repeat averages over grid
                        this.muA_mlon = grid.LonGrid;
                        this.muA_mlat = grid.LatGrid;
                        this.muA_mdat = Repeat2D(muA, gRows, gCols);
                        this.YM_mdat = Repeat2D(Young_Mod_mean, gRows, gCols);
                        this.MT_mdat = Repeat2D(TAU_MXWyr, gRows, gCols);

                        break;
                    }

                // With lateral variation
                case false:
                    {
                        // Set empty catching layers
                        Dictionary<string, double[,]> dA_Layers = new();
                        Dictionary<string, double[,]> muA_Layers = new();
                        Dictionary<string, double[,]> YM_Layers = new();
                        Dictionary<string, double[,]> WGT_Layers = new();


                        // Get data grid
                        GridLayer_PM13_T grid = GridLayer_PM13_T.Upload_Grid(z_PM13_2use[0], REGION_muA_LV);
                        this.muA_mlon = grid.LonGrid;
                        this.muA_mlat = grid.LatGrid;


                        // Iterate over depths
                        int countLyr = 0;
                        for (int k1 = 0; k1 < z_PM13_2use.Length; k1++)
                        {
                            countLyr++;
                            Console_Banners.WriteReports(5, k1, z_PM13_2use.Length);


                            // --- Upload and crop grid data ---
                            grid = GridLayer_PM13_T.Upload_Grid(z_PM13_2use[k1], REGION_muA_LV);
                            double[,] mT = grid.TempGrid;


                            int mRows = grid.gRows;
                            int mCols = grid.gCols;



                            // --- Define weights on basis of area ---
                            var dA = new double[mRows, mCols];

                            // Account for depth within the Earth (more depth, less area) 
                            var const1 = Math.Pow((Re - z_PM13_2use[k1]), 2);

                            for (int ji = 0; ji < mRows; ji++)
                                for (int jj = 0; jj < mCols; jj++)
                                {
                                    // Area represented by each grid point (in m^2)
                                    dA[ji, jj] = const1 * (-Math.Cos((90 - this.muA_mlat[ji, jj]) * (Math.PI / 180) + (dlat / 2)) +
                                                            Math.Cos((90 - this.muA_mlat[ji, jj]) * (Math.PI / 180) - (dlat / 2))) * dlon;
                                }

                            // Set weights based on area
                            double dASum = Sum(dA);
                            double[,] WGT = ArrayDivide(dA, dASum);


                            // Store area grid 
                            dA_Layers.Add(string.Format("dA_L{0}", countLyr), dA);



                            // --- Pressure at layer ---

                            // Filter PREM layers to 'PREM_z <= current depth'
                            int[] PREM2use_keys = PREM_z
                                .Select((v, i) => new { v, i })
                                .Where(p => p.v <= z_PM13_2use[k1])
                                .Select(p => p.i)
                                .ToArray();


                            // Filter layer's depth and rho values
                            double[] PREM_z_2use = SubArrayIndexes(PREM_z, PREM2use_keys);
                            double[] PREM_rho_2use = SubArrayIndexes(PREM["Rho"], PREM2use_keys);

                            PREM_z_2use = PREM_z_2use
                                .Concat(new double[1] { z_PM13_2use[k1] })
                                .ToArray();


                            // Sum pressure 'P = rho * h * g'
                            double P = ArrayMultiply(ArrayProduct(Utils.Difference(PREM_z_2use), PREM_rho_2use), g).Sum();



                            // --- Viscosity by requiring 'average == muA' ---

                            // Activation parameter
                            double[,] a = new double[mRows, mCols];

                            // PM13 Eq.1 (note lack of minus sign)
                            for (int si = 0; si < mRows; si++)
                                for (int sj = 0; sj < mCols; sj++)
                                    a[si, sj] = Math.Exp((E + P * Va) / (R * (mT[si, sj] + CK_offset)));


                            // PM13 Eq.7 (note multiplication instead of division)
                            double c = muA / Sum(ArrayProduct(WGT, a));
                            double[,] muA_layer = ArrayMultiply(a, c);

                            muA_Layers.Add($"muA_L{countLyr}", muA_layer);



                            // --- Young's modulus based on dT ---

                            // Mean layer temperature
                            double Tmean = Sum(ArrayProduct(WGT, mT));
                            double[,] mdat_dT = ArraySubstract(mT, Tmean);
                            double[,] mdat_YM = ArraySummate(ArrayMultiply(mdat_dT, dYMdT), Young_Mod_mean);

                            YM_Layers.Add($"YM_L{countLyr}", mdat_YM);
                        }


                        // --- Position weights with depth ---
                        double[,] NORM_MTX = ArraySummate((from dA_Lx in dA_Layers select dA_Lx.Value).ToArray());

                        foreach (KeyValuePair<string, double[,]> dA_Lx in dA_Layers)
                        {
                            double[,] WGT_L = ArrayDivide(dA_Lx.Value, NORM_MTX);
                            WGT_Layers.Add(string.Format("WGT_L{0}", dA_Lx.Key.Substring(4, 1).ToString()), WGT_L);
                        }


                        // === Depth-average viscosity ===
                        List<double[,]> muA_AVGs = new();

                        for (int k = 1; k <= z_PM13_2use.Length; k++)
                            muA_AVGs.Add(ArrayProduct(muA_Layers[$"muA_L{k}"], WGT_Layers[$"WGT_L{k}"]));


                        this.muA_mdat = ArraySummate(muA_AVGs.ToArray());



                        // === Depth-average Young's Modulus ===
                        List<double[,]> YM_AVGs = new();

                        for (int k = 1; k <= z_PM13_2use.Length; k++)
                            YM_AVGs.Add(ArrayProduct(YM_Layers[$"YM_L{k}"], WGT_Layers[$"WGT_L{k}"]));


                        this.YM_mdat = ArraySummate(YM_AVGs.ToArray());



                        // === Depth-average of Maxwell time ===
                        this.MT_mdat = ArrayDivide(ArrayDivide(this.muA_mdat, this.YM_mdat), (365 * 24 * 60 * 60));

                        break;
                    }
            }
        }


        /// <summary>
        /// Save the grids into TXT files.
        /// </summary>
        /// <param name="modelName">String used for file naming. Should usually provide information on the parameters used for the particular asthenosphere model.</param>
        /// <param name="dirPath">Path to the directory that will host the files.</param>
        public void Save_AverageGrids(string modelName, string dirPath)
        {
            // --- Save grid values for muA (Asthenosphere's Viscosity), YM (Young's Module) and TXT file ---
            ManageOutputs.Set_GridValues_FileNames(modelName,
                out string GRID_LON_FILENAME, out string GRID_LAT_FILENAME,
                out string GRID_MuA_FILENAME, out string GRID_YM_FILENAME,
                out string GRID_MT_FILENAME);

            ManageOutputs.Save_toTXT(muA_mlon, dirPath, GRID_LON_FILENAME, "0.0");
            ManageOutputs.Save_toTXT(muA_mlat, dirPath, GRID_LAT_FILENAME, "0.0");
            ManageOutputs.Save_toTXT(muA_mdat, dirPath, GRID_MuA_FILENAME);
            ManageOutputs.Save_toTXT(YM_mdat, dirPath, GRID_YM_FILENAME);
            ManageOutputs.Save_toTXT(MT_mdat, dirPath, GRID_MT_FILENAME);
        }

        private static void Upload_PremModel(out Dictionary<string, double[]> PREM_model)
        {
            // Data from Dziewonski, A.M., and D. L.Anderson. 1981.
            // "Preliminary reference Earth model." Phys.Earth Plan. Int. 25:297 - 356.

            // Read Prem_MantleModel file as lines (skip first)
            string s = Properties.TextFiles.Prem_MantleModel;
            string[] lines = s.Split("\r\n");
            lines = lines.Skip(1).ToArray();


            // Store values in 2D array
            double[,] prem = new double[lines.Length, 9];

            for (int i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < values.Length; j++)
                {
                    prem[i, j] = GetValue(values[j]);
                }
            }

            // Store values in Dictionary
            PREM_model = new()
            {
                // radius(km) alpha(m/s) beta(m/s) rho(kg/m**3) Ks(GPa) mu(GPa) gamma  Pres(Gpa) g(m/s**2)

                { "Radius", GetColumn(prem, 0) },
                { "Alpha",  GetColumn(prem, 1) },
                { "Beta",   GetColumn(prem, 2) },
                { "Rho",    GetColumn(prem, 3) },
                { "Ks",     GetColumn(prem, 4) },
                { "Mu",     GetColumn(prem, 5) },
                { "Gamma",  GetColumn(prem, 6) },
                { "Pres",   GetColumn(prem, 7) },
                { "g",      GetColumn(prem, 8) },
            };
        }

        /// <summary>
        /// Best fit for the relationship between thickness and viscosity 
        /// contrast of the asthenosphere, based on Paulson and Richards 2009.
        /// </summary>
        /// <param name="nSamples">Number of samples to build.</param>
        /// <returns>Ensemble of size <paramref name="nSamples"/> for values
        /// of A, the proportionality constant from Paulson and Richards 2009.
        /// </returns>
        private static double[] PR09_F3_DATA_FIT(int nSamples)
        {
            // Digitalized points from Paulson & Richards Fig3
            double[] x = new double[4] { 0.73346, 1.28794, 1.47276, 2.23476 };  // x = log10(muM / muA);
            double[] y = new double[4] { 271.739, 177.002, 153.661, 85.0114 };  // y = HA[km]
            double[] yl = new double[4] { 260.069, 166.018, 143.021, 72.6545 }; // yl = y's lower limit
            double[] yu = new double[4] { 282.037, 189.359, 166.018, 96.3387 }; // yu = y's upper limit


            // Log arrays
            double Log10(double item) => Math.Log10(item * 1e3);
            double[] yLog = ApplyFunction(y, Log10);
            double[] ylLog = ApplyFunction(yl, Log10);
            double[] yuLog = ApplyFunction(yu, Log10);


            int nPoints = x.Length;


            // Repeat Array x as nSamples-size columns in Matrix Mx 
            double[,] Mx = Repeat1DArray(x, 0, nSamples, 1);


            // Generate Matrix My from ylLog, yuLog and random values
            double[][] randomArray = ToArrayOfArrays(Utils.Random2D(nPoints, nSamples));
            double[][] My = new double[nPoints][];

            for (int i = 0; i < nPoints; i++)
            {
                My[i] = ArrayMultiply(randomArray[i], Math.Abs(ylLog[i] - yuLog[i]));
                My[i] = ArraySummate(My[i], Math.Min(ylLog[i], yuLog[i]));
            }


            // Generate Array Vq from the summation of Matrices My and 1/3Mx
            double[] Vq = new double[nSamples];
            for (int k1 = 0; k1 < nSamples; k1++)
            {
                double temp = 0;
                for (int j1 = 0; j1 < nPoints; j1++)
                    temp += My[j1][k1] + 1.0 / 3.0 * Mx[j1, k1];

                Vq[k1] = (double)1 / nPoints * temp;
            }


            // Ensemble of A values
            double[] A_SAMPLES = (from value in Vq select Math.Pow(10, value)).ToArray();


            return A_SAMPLES;
        }

    }
}
