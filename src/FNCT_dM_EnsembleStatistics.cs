﻿using static MYRIAM.ManageOutputs;
using EnsembleAnalysis;
using StructOperations;
using Histograms;
using Torque;
using ContourBuilder;
using Cartography;
using DataStructures;


namespace MYRIAM
{
    internal class FNCT_dM_EnsembleStatistics
    {
        public static void dMPole_Contours(TorqueVector[] dM, double[,] RMTX,
                                           double[] DM_CNTR_BINS, double[] DM_CNTR_PERCENT,
                                           int stageIndex_Old, int stageIndex_Young, 
                                           string dir_dM_PDD, string dir_TMP,
                                           string mtxLabel, out double[] DM_BINS_OUT,
                                           out double[] ANG_R)
        {
            // Store temporal text file with dM ensemble
            string NAME_ENSdM = $"ENSdM.txt";
            Save_toTXT(dM, dir_TMP, NAME_ENSdM, format: "#.###E+0");


            // --- Rotate ensemble points (contouring fails at poles) ---

            double[,] RMTX_2use = RMTX;


            // Set angles of rotation to the given angle sin RMTX
            ANG_R = new double[3];


            // Use alternative matrix if RMTX is not explicitly set
            if (MatrixOperations.Matrix_IsNaN(RMTX))
                RMTX_2use = getOptimal_rotMatrix(dM, out ANG_R);


            // Apply rotation to ensemble
            TorqueVector[] dM_rot = TorqueVector.VectorProduct(dM, RMTX_2use);
            dM = new TorqueVector[0];


            // Store temporal text file with dM rotated ensemble
            string NAME_ENSdM_ROT = $"ENSdM_ROT.txt";
            Save_toTXT(dM_rot, dir_TMP, NAME_ENSdM_ROT, format: "#.###E+0");



            // --- Extract dM Pole arrays ---

            // Transform cartesian coordinates to spherical degrees coordinates
            dM_rot = TorqueVector.ToSpherical(dM_rot);


            // Store coordinates columns in 1D arrays 
            TorqueVector.GetSphericalColumns(dM_rot, out double[] colLon, out double[] colLat, out _); // !!! chnage sintax?
            dM_rot = new TorqueVector[0];


            // Dump storage
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // =========== Extract dM Contours ==============================

            // Calculate 2D histogram and extract contours
            Contour[] contourArray = Pole_Statistics.Extract_PoleContours(
                colLon, colLat, DM_CNTR_PERCENT, DM_CNTR_BINS, out DM_BINS_OUT);
            colLon = new double[0];
            colLat = new double[0];


            // Dump storage
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Store temporal text file with contour 
            string NAME_CNTR_ROT = $"CNTR_ROT_68.txt";
            Coordinate[] cntr68 = contourArray.
                Where(x => x.PercentInterval == 68).
                Select(x => x.Coordinates).
                ToArray()[0];

            Save_toTXT(cntr68, dir_TMP, NAME_CNTR_ROT, format: "F1");


            // --- Rotate contours back ---

            // Calculate the inverse of RMTX_2use
            double[,] RMTX_INV = MatrixOperations.MatrixInverse(RMTX_2use);


            // Rotate each percentage contour
            for (int i = 0; i < contourArray.Length; i++)
                contourArray[i].Coordinates = rotateCoordinate(contourArray[i].Coordinates, RMTX_INV);


            // --- Save contour coordinates as txt file ---

            foreach (Contour contour in contourArray)
            {
                string CNTR_PDD_LBL = Set_ContourCoordinates_FileName(
                    contour.PercentInterval, stageIndex_Old, stageIndex_Young, mtxLabel, DM_BINS_OUT[0]);

                Save_toTXT(contour.Coordinates, dir_dM_PDD, CNTR_PDD_LBL);
            }


            // Store temporal text file with contour 
            string NAME_CNTR = $"CNTR_68.txt";

            Coordinate[] cntr68_ROT = contourArray.
                Where(x => x.PercentInterval == 68).
                Select(x => x.Coordinates).
                ToArray()[0];

            Save_toTXT(cntr68_ROT, dir_TMP, NAME_CNTR, format: "F1");
        }


        public static void dMMag_Histogram(TorqueVector[] dM, int nBins, 
                                           int stageIndex_Old, int stageIndex_Young, string dir_dM_PDD,
                                           string mtxLabel, out int DM_MAGHIST_OUT)
        {
            // --- Extract dM Magnitude array ---

            // Transform cartesian coordinates to spherical degrees coordinates
            dM = TorqueVector.ToSpherical(dM);


            // Store coordinates columns in 1D arrays 
            TorqueVector.GetSphericalColumns(dM, out _, out _, out double[] magArray);
            dM = new TorqueVector[0];


            // Dump storage
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // --- Construct Histogram ---


            // Calculate histogram
            Histogram hist = HistogramBuilder.MakeHistogram(magArray, nBins);
            magArray = new double[0];


            // Dump storage
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Set bin Value and Count array
            double[] binCount = (from values in hist.Values select Convert.ToDouble(values)).ToArray();
            double[] binMidValues = hist.MidBins;
            double[,] magHist_XY = ArrayManagement.To2DArray(binMidValues, binCount);


            // Store histogram metrics
            DM_MAGHIST_OUT = (int) hist.Length;


            // Save dM's magnitude histogram outline as txt file
            string MAG_HIST_LBL = Set_MagnitudeHistogram_FileName(
                stageIndex_Old, stageIndex_Young, mtxLabel, DM_MAGHIST_OUT);

            Save_toTXT(magHist_XY, dir_dM_PDD, MAG_HIST_LBL, new string[] { "#.###E+0", "F0" });
        }


        /// <summary>
        /// Given an ensemble of cartesian coordinates, this function
        /// takes the average longitude and latitude to construct a rotation
        /// matrix that centers the ensemble on the 0N, 0E.
        /// </summary>
        /// <param name="ens">Array of cartesian coordinates.</param>
        /// <param name="ANG_R">Array that holds the rotation angles 
        /// used for the axes Val, Lat and Lon.</param>
        /// <returns>Rotation matrix.</returns>
        private static double[,] getOptimal_rotMatrix(TorqueVector[] ens, 
            out double[] ANG_R)
        {
            // Average vector in spherical coordinates
            TorqueVector dEVmean = Ensemble_Statistics.EnsembleMean(ens);


            // Convert mean pole to degrees
            double meanLon = Math.Round(dEVmean.Longitude * (180 / Math.PI), 1);
            double meanLat = Math.Round(dEVmean.Latitude * (180 / Math.PI), 1);


            // Set rotation angles (for output)
            ANG_R = new double[] { meanLon, meanLat, 0.0 }; 


            // Multiply rotation matrices
            return Build_RotationMatrix.Set_RotationMatrix(-meanLon, meanLat);
        }


        /// <summary>
        /// Rotates a given ensemble of spherical xy-coordinates, 
        /// applying the given a rotation matrix.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="rotMatrix"></param>
        /// <returns></returns>
        private static Coordinate[] rotateCoordinate(Coordinate[] coordinates, double[,] rotMatrix)
        {
            // Turn spherical coordinates to cartesian
            Vector[] cntrCart = coordinates.Select(x => x.ToCartesian()).ToArray();

            // Rotate cartesian coordiantes
            TorqueVector[] cntrCartInv = (TorqueVector[])TorqueVector.VectorProduct(cntrCart, rotMatrix);

            // Transform back to spherical coordinates
            Coordinate[] cntrSph = cntrCartInv.Select(x => Coordinate.ToSpherical(x)).ToArray();

            return cntrSph;
        }
    }
}
