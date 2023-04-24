using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructOperations;
using DataStructures;
//using System.Data;        !!! is it used?
using System.Drawing;
using System.Reflection.Emit;
using System.Reflection;
using static MYRIAM.FileReader;
using static StructOperations.ArrayOperations;
using static StructOperations.ArrayManagement;
using Cartography;
using Utilities;


namespace MYRIAM
{
    internal class FNCT_muA_MAPPER
    {
        /// <summary>
        /// This function calculates the average thickness of the asthenosphere channel 
        /// and grids for the asthenosphere's depth-average viscosity and Young's modulus.
        /// </summary>
        /// <param name="muM">Average viscosity of the upper mantle in Pa*s.</param>
        /// <param name="muA">Average viscosity of the asthenosphere in Pa*s.</param>
        /// <param name="HL">Maximun thickness of the lithosphere in meters.</param>
        /// <param name="REGION_muA_LV">Geographical limits (in degrees) whithin which <c>muA</c> is average.</param>
        /// <param name="fHA">Fraction of the upper asthenosphere channel to use.</param>
        /// <param name="HA">Output average thickness of the asthenosphere channel in meters.</param>
        /// <param name="glon">Output 2D grid with the longitude (in degrees) for each datapoint.</param>
        /// <param name="glat">Output 2D grid with the latitude (in degrees) for each datapoint.</param>
        /// <param name="gmuA">Output 2D grid with the asthenosphere viscosity (in Pa*s) for each datapoint.</param>
        /// <param name="gYM">Output 2D grid with the asthenosphere Young's modulus (in Pa) for each datapoint.</param>
        public static void muA_MAPPER(double muM, double muA, double HL, CartoLimits REGION_muA_LV, double fHA, 
                                      out double HA, out double[,] glon, out double[,] glat, 
                                      out double [,] gmuA, out double[,] gYM, out double[,] gMT)
        {
            /* Starting from an input value of the desired asthenosphere (muA [Pa*s]) and 
             * upper mantle (muM [Pa*s]) average-viscosities, this script calculates the 
             * expected thickness of the asthenosphere following Paulson & Richards (2009). 
             * Alltogether with (i) an input value for the maximum thickness of the lithosphere 
             * (HL [m]), (ii) asthenopshere's temperature extracted from the model of
             * Priestley & McKenzie (2013), and (iii) pressures values at various depth 
             * calculated from the PREM model, this script also calculates a power-law 
             * coefficient for the viscosity(c) that would yield a layer-average equal to 
             * muA in input. This is done building on equations 15-17 of Priestley & McKenzie (2013). 
             * The viscosity at any point p is calculated as muA(p) = c * exp( (E+P(p) Va) / (RT(p)) ).
             * Once the layer viscosity has been calculated for all model layers that fall 
             * within the asthenosphere, an output depth average at all positions is calculated 
             * (HA [m]), as well as 2D arrays storing the grid data points values for longitude 
             * (glon [degrees]), latitude (glat [degrees]), and asthenosphere viscosity (gmuA [Pa*s]).
             * Ouput grid values for Young's modulus (gYM) are also obtained using the relationship
             * in Priestley & McKenzie (2013), but accounting only for temperature lateral variations
             * - since the pressure at any depth is uniform and calculated from PREM.
             * 
             * All the above is performed within the geographical rectangular-region specified 
             * through the input <paramref name="REGION_muA_LV"/>. Furthermore, one may elect to only 
             * use the upper portion of the asthenosphere specified through the input "fHA" 
             * (between 0 and 1) in order to perform the depth averaging at any position. 
             */

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
            double CK_offset = 273.15; // !!! What is it? Where does it come from

            // Young's modulus [in Pa], based on Mantle from Turcotte and Schubert
            double Young_Mod_mean = 1.5e11; 

            // Derivative of Young modulus with temperature !!! [Pa/K]? From PM13?
            double dYMdT = -0.87 * 1e-2 * 1e9; 
            
            // Maxwell time (in years)
            double TAU_MXWyr = (muA / Young_Mod_mean) / (365 * 24 * 60 * 60); 



            // === Calculate thickness of asthenosphere according to PR09 === 

            // Sample probability constant (A) for D = A * (muA/muM)^(1/3)
            double[] A_samples = FNCT_PR09_F3_DATA_FIT.PR09_F3_DATA_FIT(nSamples_PR09);


            // Calculate asthenosphere thickness from muA and muU (Paulson & Richards, 2009)
            double[] HA_SAMPLES = ArrayMultiply(A_samples, Math.Pow(muA / muM, 1.0 / 3.0));
            double HA_PR09 = HA_SAMPLES.Average();
            HA = HA_PR09;
            


            // === Layer viscosity on the basis of temperature lateral variations ===

            // Filter layers to depths of asphenosphere (HL <= p <= (HL + HA_PR09 * fHA)) 

            double[] z_PM13_1e3 = Utils.Arange(50e3, 400e3, 25e3).ToArray(); // Available PM13 depths (every 25km)
            double[] z_PM13_2use = z_PM13_1e3.Where(p => p >= HL && p <= (HL + HA_PR09 * fHA)).ToArray();


            // Throw warnings and errors
            if (HL + HA > 400e3)
                Console.WriteLine(
                    "Warning! " +
                    "Input HL and calculated HA yield an added thickness larger than 400 km (max. depth of PM13).");

            if (z_PM13_2use.Length == 0)
                throw new ArgumentException(
                    "Error! " +
                    "Combination of given HL and fHA yield a layer of thickness smaller " +
                    "than the resolution of model PM_v2_2012 (from 50 to 400 km every 25 km).");


            Console_Banners.WriteReports(4);
            switch (REGION_muA_LV.IsEmpty())
            {
                // No lateral variation
                case true:
                    {
                        // Get data grid
                        Upload_GRDLYR_PM13_T(z_PM13_2use[0], new CartoLimits().SetGlobal(), 
                            out double[,] mlon, out double[,] mlat, out _);

                        glon = mlon;
                        glat = mlat;

                        int gRows = mlon.GetLength(0);
                        int gCols = mlon.GetLength(1);


                        // Repeat averages over grid
                        double[,] muA_AVG = Repeat2D(muA, gRows, gCols);
                        double[,] YM_AVG = Repeat2D(Young_Mod_mean, gRows, gCols);
                        double[,] MT_AVG = Repeat2D(TAU_MXWyr, gRows, gCols);

                        gmuA = muA_AVG;
                        gYM = YM_AVG;
                        gMT = MT_AVG;

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
                        Upload_GRDLYR_PM13_T(z_PM13_2use[0], REGION_muA_LV,
                            out double[,] mlon, out double[,] mlat, out _);

                        glon = mlon;
                        glat = mlat;


                        // Iterate over depths
                        int countLyr = 0;
                        for (int k1 = 0; k1 < z_PM13_2use.Length; k1++)
                        {
                            countLyr++;
                            Console_Banners.WriteReports(5, k1, z_PM13_2use.Length);


                            // --- Upload and crop grid data ---
                            Upload_GRDLYR_PM13_T(z_PM13_2use[k1], REGION_muA_LV, out _, out _, out double[,] mT);

                            int mRows = mT.GetLength(0);
                            int mCols = mT.GetLength(1);



                            // --- Define weights on basis of area ---
                            var dA = new double[mRows, mCols];

                            // Account for depth within the Earth (more depth, less area) 
                            var const1 = Math.Pow((Re - z_PM13_2use[k1]), 2);

                            for (int ji = 0; ji < mRows; ji++)
                                for (int jj = 0; jj < mCols; jj++)
                                {
                                    // Area represented by each grid point (in m^2)
                                    dA[ji, jj] = const1 * (-Math.Cos( (90 - mlat[ji, jj]) * (Math.PI / 180) + (dlat / 2) ) +
                                                            Math.Cos( (90 - mlat[ji, jj]) * (Math.PI / 180) - (dlat / 2) )) * dlon;
                                }

                            // Set weights based on area
                            double dASum = Sum(dA);
                            double[,] WGT = ArrayDivide(dA, dASum);


                            // Store area grid 
                            dA_Layers.Add(string.Format("dA_L{0}", countLyr), dA);



                            // --- Pressure at layer ---

                            // Filter PREM layers to 'PREM_z <= current z_PM13_2use'
                            int[] PREM2use_keys = PREM_z
                                .Select((v, i) => new { v, i })
                                .Where(p => p.v <= z_PM13_2use[k1])
                                .Select(p => p.i)
                                .ToArray();


                            // Filter layer's z and rho values
                            double[] PREM_z_2use = SubArrayIndexes(PREM_z, PREM2use_keys);
                            double[] PREM_rho_2use = SubArrayIndexes(PREM["Rho"], PREM2use_keys);

                            PREM_z_2use = PREM_z_2use
                                .Concat(new double[1] { z_PM13_2use[k1] })
                                .ToArray();


                            // Summate pressure 'P = rho * h * g'
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

                        gmuA = ArraySummate(muA_AVGs.ToArray());


                        // === Depth-average Young's Modulus ===
                        List<double[,]> YM_AVGs = new();

                        for (int k = 1; k <= z_PM13_2use.Length; k++)
                            YM_AVGs.Add(ArrayProduct(YM_Layers[$"YM_L{k}"], WGT_Layers[$"WGT_L{k}"]));

                        gYM = ArraySummate(YM_AVGs.ToArray());


                        // === Depth-average of Maxwell time ===
                        gMT = ArrayDivide(ArrayDivide(gmuA, gYM), (365 * 24 * 60 * 60));
                        
                        break;
                    }
            }
        }


        private static void Upload_PremModel(out Dictionary<string, double[]> PREM_model)
        {
            // Data from Dziewonski, A.M., and D. L.Anderson. 1981.
            // "Preliminary reference Earth model." Phys.Earth Plan. Int. 25:297 - 356.

            // Read Prem_MantleModel file as lines (skip first)
            string pathPrem = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                @"..\..\..\assets\TextFiles\Prem_MantleModel.txt");

            string[] lines = File.ReadAllLines(pathPrem);
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


        private static void Upload_GRDLYR_PM13_T(double z_PM13_2use, CartoLimits REGION_muA_LV,
                                                 out double[,] mlon, out double[,] mlat, out double[,] mT )
        {
            // Set depth string
            string DPT;
            if (z_PM13_2use < 100e3)
                DPT = $"0{z_PM13_2use / 1e3}";
            else
                DPT = $"{z_PM13_2use / 1e3}";


            // Read Prem_MantleModel file as lines
            string premDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\assets\TextFiles\PriestleyMcKenzey2013_T_model");

            string filePath = Path.Combine(premDir, "GRD_PM13_DEPTH" + DPT + "km_T.txt");
            string[] lines = File.ReadAllLines(filePath);


            // Set empty 2D arrays
            double[,] GRD_PM13_mlon = new double[90, 180];
            double[,] GRD_PM13_mlat = new double[90, 180];
            double[,] GRD_PM13_mdat = new double[90, 180];


            // Fill arrays with parsed values
            for (int i = 0; i < lines.Length; i++)
            {
                int k = i/180;
                var values = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < values.Length; j++)
                {
                    if (j == 0) GRD_PM13_mlon[k, i-180*k] = GetValue(values[j]);
                    if (j == 1) GRD_PM13_mlat[k, i-180*k] = GetValue(values[j]);
                    if (j == 2) GRD_PM13_mdat[k, i-180*k] = GetValue(values[j]);
                }
            }

            Crop_GRD_PM13(GRD_PM13_mlon, GRD_PM13_mlat, GRD_PM13_mdat, REGION_muA_LV, out mlon, out mlat, out mT);
        }

        private static void Crop_GRD_PM13( double[,] GRD_PM13_mlon, double[,] GRD_PM13_mlat, double[,] GRD_PM13_mdat, CartoLimits SQUARE,
                                          out double[,] mlon, out double[,] mlat, out double[,] mdat )
        {
            // --- mlon filters ---
            double[] grdCols = GetRow(GRD_PM13_mlon, 0);
            var grdCols_pair = grdCols
                .Select((element, index) => new KeyValuePair<double, double>(index, element))
                .Where(p => p.Value >= SQUARE.LonMin && p.Value <= SQUARE.LonMax)
                .ToArray();


            var grdCols_keys = (from value in grdCols_pair select (int)value.Key).ToArray();


            // --- mlat filters ---
            double[] grdRows = GetColumn(GRD_PM13_mlat, 0);
            var grdRows_pair = grdRows
                .Select((element, index) => new KeyValuePair<double, double>(index, element))
                .Where(p => p.Value >= SQUARE.LatMin && p.Value <= SQUARE.LatMax)
                .ToArray();

            var grdRows_keys = (from value in grdRows_pair select (int)value.Key).ToArray();


            // --- Crop arrays ---
            mlon = new double[grdRows_keys.Length, grdCols_keys.Length];
            mlat = new double[grdRows_keys.Length, grdCols_keys.Length];
            mdat = new double[grdRows_keys.Length, grdCols_keys.Length];

            for (int ui=0; ui< grdRows_keys.Length; ui++)
                for (int uj = 0; uj < grdCols_keys.Length; uj++)
                {
                    mlon[ui, uj] = GRD_PM13_mlon[grdRows_keys[ui], grdCols_keys[uj]];
                    mlat[ui, uj] = GRD_PM13_mlat[grdRows_keys[ui], grdCols_keys[uj]];
                    mdat[ui, uj] = GRD_PM13_mdat[grdRows_keys[ui], grdCols_keys[uj]];
                }
        }
    }
}
