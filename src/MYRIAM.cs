using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using StructOperations;
using static MYRIAM.Console_Banners;
using static MYRIAM.ManageOutputs;
using MYRIAM.Torque;


namespace MYRIAM
{
    class EntryPoint
    {
        static public void Main(string[] args)
        {
            // --- Openning Console Banner ---
            MainBanner();
            WriteReports(1);


            // ============== Parameters ===========================================

            string inputFilePath;

            // Check if input file path was given in the prompt
            try
            {
                inputFilePath = args[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InputErrorException(
                    "Error in console syntax. " +
                    "The software call must be followed by the path to the inputs textfile.");
            }

            // Set run parameters
            InputParameters inputParams = Manage_InputParams.Set_InputParams(inputFilePath);


            // Main inputs banner
            InputSummaryBanner(inputParams);



            // ============== Set output dirs and file labels ======================

            // Create output directories
            WriteReports(2);
            Create_OutputDirs(inputParams,
                out string dir_MTXwM, out string dir_dM_PDD, out string dir_TMP);


            // Set up model and matrix labels
            string modelName = Set_Model_Label(inputParams);
            string mtxLabel = Set_Matrix_Label(inputParams, modelName);



            // ================== First Main Equation ==============================
            // Euler vector(w) to torque(M) with lateral variations of asthenosphere
            // viscosity(muA)

            Calculate_dEV2dM_Matrix.w2M_MTX_CorLV_muA(inputParams, dir_MTXwM, modelName);



            // ================== Second Main Equation =============================
            // Calculates the torque-variation (dM) associated with an Euler-vector
            // change(dEV) of a given tectonic plate

            FNCT_EVdM.EV_to_dM(
                inputParams, dir_MTXwM, dir_dM_PDD, mtxLabel, 
                out TorqueVector[] dM, out TorqueVector dMvector);


            OutputSummaryBanner(dMvector);



            // ================== Optional dM Plotting calculations ================
            // Calculate the distributions of magnitude (histogram) and
            // pole (confidence contour), and output them via TXT files.
            
            if (inputParams.ContainsKey("DM_MAGHIST_BINS"))
            {
                WriteReports(12, 1);

                dM_EnsembleStatistics.Magnitude_Histogram(
                    inputParams, dM, dir_dM_PDD, mtxLabel,
                    out int DM_MAGHIST_OUT);

                // Update used parameters
                inputParams.DM_MAGHIST_BINS = DM_MAGHIST_OUT;
            }
            else
                WriteReports(12, 0);


            if (inputParams.ContainsKey("DM_CNTR_BINS"))
            {
                WriteReports(13, 1, 0);

                // Contour parameters
                double[] DM_CNTR_BINS = inputParams.DM_CNTR_BINS;
                double[] DM_CNTR_PERCENT = inputParams.DM_CNTR_PERCENT;
                double[] ANG_R = new double[3];


                // Set angles of main axes rotations
                if (inputParams.ContainsKey("ANG_ROT"))
                    ANG_R = inputParams.ANG_ROT;
                else
                    ANG_R = ArrayManagement.Repeat1D(double.NaN, 3);



                // Set up rotation matrix for contour ellipse calculation improvement
                double[,] ROT_MTX = Build_RotationMatrix.Set_RotationMatrix(ANG_R[0], ANG_R[1], ANG_R[2]);


                // === Calculate Contours ===
                dM_EnsembleStatistics.Pole_Contours(
                    inputParams, dM, ROT_MTX, DM_CNTR_BINS, DM_CNTR_PERCENT,
                    dir_dM_PDD, dir_TMP, mtxLabel,
                    out double[] DM_CNTR_OUT, out double[] ANG_R_OUT);


                // Update used parameters
                inputParams.ANG_ROT = ANG_R_OUT;
                inputParams.DM_CNTR_BINS = DM_CNTR_OUT;
            }
            else
                WriteReports(13, 0);




            // Empty large variables
            dM = new TorqueVector[0];
            GC.Collect();
            GC.WaitForPendingFinalizers();            
            

            // --- Save parameters Report ---
            Save_InputParamsReport(inputParams);



            // ================== Optional Output Figures ==========================
            // Generate figures using Python

            if (inputParams.PYTHON_PATH != null)
            {
                WriteReports(14, 1);

                string pythonPath = inputParams.PYTHON_PATH;


                // Generate Asthenosphere-Viscosity, Youngs-Modulus and Maxwell-time maps
                Parallel.Invoke(
                        () => WriteReports(15, 0),
                        () => Generate_gridFigures(pythonPath, dir_MTXwM, modelName, inputParams.PLT_LABEL)
                        );


                // Generate Torque variation magnitude histogram
                if (inputParams.ContainsKey("DM_MAGHIST_BINS"))
                {
                    int nBins = inputParams.DM_MAGHIST_BINS;
                    string histLabel = Set_MagnitudeHistogram_Label(
                        inputParams.STG_IDX_1, inputParams.STG_IDX_2, mtxLabel, nBins);

                    Parallel.Invoke(
                        () => WriteReports(15, 1, 1),
                        () => Generate_histogramFigure(pythonPath, dir_dM_PDD, histLabel)
                        );
                }
                else
                    WriteReports(15, 1, 0);


                // Generate Torque variation pole distribution maps
                if (inputParams.ContainsKey("DM_CNTR_BINS"))
                {
                    // Contour Map
                    double cntrRes = inputParams.DM_CNTR_BINS[0];
                    string contourLabel = Set_ContourCoordinates_Label(
                        inputParams.STG_IDX_1, inputParams.STG_IDX_2, mtxLabel, cntrRes);

                    Parallel.Invoke(
                        () => WriteReports(15, 2, 1),
                        () => Generate_contourFigure(pythonPath, dir_MTXwM, dir_dM_PDD, contourLabel, inputParams.PLT_LABEL)
                        );


                    if (inputParams.ContainsKey("ANG_ROT"))
                    {
                        // Rotated ensemble map
                        Parallel.Invoke(
                            () => WriteReports(15, 3, 1),
                            () => Generate_rotatedCntr_Figure(pythonPath, inputParams.PLT_LABEL, dir_TMP, dir_MTXwM, dir_dM_PDD)
                            );
                    }
                    else
                        WriteReports(15, 3, 0);
                }
                else
                    WriteReports(15, 2, 0);
            }
            else
                WriteReports(14, 0);

            Delete_TMPs(dir_TMP);
            WriteReports(16);
        }
    }
}
