using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MYRIAM.Set_OutputLabels;
using static MYRIAM.ManageOutputs;
using DataStructures;
using StructOperations;
using static MYRIAM.Console_Banners;


namespace MYRIAM
{
    class MAIN_SCRIPT
    {
        static public void Main(string[] args)
        {
            // --- Openning Console Banner ---
            MainBanner();
            WriteReports(1);


            // ============== Parameters ===============================================

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

            // Set parameters
            Dictionary<string, object> inputParams = new();
            inputParams = Manage_InputParams.Set_InputParams(inputFilePath);

            string plateLabel = (string)inputParams["PLT_LABEL"];
            string outputDir = (string)inputParams["OUTPUTS_DIR"];
            string outputLabel = (string)inputParams["OUTPUT_LABEL"];
            string EVy_Path = (string)inputParams["EVy_PATH"];
            string EVo_Path = (string)inputParams["EVo_PATH"];
            string interpMethod = (string)inputParams["INTERP_MTD"];
            string contourPath = (string)inputParams["CTR_PATH"];

            int[] STG_IDXs = (int[])inputParams["STG_IDXs"];
            int stageIndex_Old = STG_IDXs[0];
            int stageIndex_Young = STG_IDXs[1];

            double gridRes = (double)inputParams["GRID_RES"];
            double muM = (double)inputParams["muM"];
            double muA = (double)inputParams["muA"];
            double HL = (double)inputParams["HL"];
            double fHA = (double)inputParams["FRACTION_HA"];
            double defLength = (double)inputParams["DEF_DISTANCE"];

            CoordsLimits REGION_muA_LV = (CoordsLimits)inputParams["REGION_muA_LV"];
            bool saveEnsemble = (bool)inputParams["SAVE_ENS"];
            bool overwriteOutput = (bool)inputParams["OVERWRT_OUTPUT"];



            // Main inputs banner
            InputSummaryBanner(inputParams);



            // ============== Set output dirs and file labels ===============================================

            // Create output directories
            WriteReports(2);
            Create_OutputDirs(outputDir, plateLabel, outputLabel, overwriteOutput,
                out string dir_MTXwM, out string dir_dM_PDD, out string dir_TMP);


            // Set up model and matrix labels
            string modelName = Set_Model_Label(fHA, muM, muA, HL, REGION_muA_LV, defLength);
            string mtxLabel = Set_Matrix_Label(plateLabel, interpMethod, modelName);


            // ================== First Main Equation ==============================
            // Euler vector(w) to torque(M) with lateral variations of asthenosphere
            // viscosity(muA)

            FNCT_w2M_MTX_CorLV_muA.w2M_MTX_CorLV_muA(
                dir_MTXwM, contourPath, muM, muA, 
                HL * 1e3, fHA, gridRes, REGION_muA_LV, 
                interpMethod, defLength * 1e3, plateLabel, modelName);



            // ================== Second Main Equation =============================
            // Calculates the torque-variation (dM) associated with an Euler-vector
            // change(dEV) of a given tectonic plate

            FNCT_EVdM.EV_to_dM(
                stageIndex_Old, stageIndex_Young, EVo_Path, EVy_Path,
                dir_MTXwM, dir_dM_PDD, mtxLabel, saveEnsemble,
                out vectorCart[] dM, out VectorDegCov dMvector);


            OutputSummaryBanner(dMvector);



            // ================== Optional dM Plotting calculations ==========================

            
            if (inputParams.ContainsKey("DM_MAGHIST_BINS"))
            {
                WriteReports(12, 1);

                int DM_MAGHIST_BINS = (int)inputParams["DM_MAGHIST_BINS"];

                FNCT_dM_EnsembleStatistics.dMMag_Histogram(
                    dM, DM_MAGHIST_BINS, stageIndex_Old, stageIndex_Young, dir_dM_PDD, mtxLabel,
                    out int DM_MAGHIST_OUT);

                // Update used parameters
                inputParams["DM_MAGHIST_BINS"] = DM_MAGHIST_OUT;
            }
            else
                WriteReports(12, 0);


            if (inputParams.ContainsKey("DM_CNTR_BINS"))
            {
                WriteReports(13, 1, 0);

                // Contour parameters
                double[] DM_CNTR_BINS = (double[])inputParams["DM_CNTR_BINS"];
                double[] DM_CNTR_PERCENT = (double[])inputParams["DM_CNTR_PERCENT"];
                double[] ANG_R = new double[3];


                // Set angles of main axes rotations
                if (inputParams.ContainsKey("ANG_ROT"))
                    ANG_R = (double[])inputParams["ANG_ROT"];
                else
                    ANG_R = ArrayManagement.Repeat1D(double.NaN, 3);



                // Set up rotation matrix for contour ellipse calculation improvement
                double[,] ROT_MTX = Build_RotationMatrix.Set_RotationMatrix(ANG_R[0], ANG_R[1], ANG_R[2]);


                // === Calculate Contours ===
                FNCT_dM_EnsembleStatistics.dMPole_Contours(
                    dM, ROT_MTX, DM_CNTR_BINS, DM_CNTR_PERCENT,
                    stageIndex_Old, stageIndex_Young, dir_dM_PDD, dir_TMP, mtxLabel,
                    out double[] DM_CNTR_OUT, out double[] ANG_R_OUT);


                // Update used parameters
                inputParams["ANG_ROT"] = ANG_R_OUT;
                inputParams["DM_CNTR_BINS"] = DM_CNTR_OUT;
            }
            else
                WriteReports(13, 0);




            // Empty large variables
            dM = new vectorCart[0];
            GC.Collect();
            GC.WaitForPendingFinalizers();            
            

            // --- Save parameters Report ---
            Save_InputParamsReport(inputParams);



            // ================== Optional Output Figures ==========================
            if (inputParams["PYTHON_PATH"] != null)
            {
                WriteReports(14, 1);

                string pythonPath = (string)inputParams["PYTHON_PATH"];


                // Generate Asthenosphere-Viscosity, Youngs-Modulus and Maxwell-time maps
                Parallel.Invoke(
                        () => WriteReports(15, 0),
                        () => Generate_gridFigures(pythonPath, dir_MTXwM, modelName, plateLabel)
                        );


                // Generate Torque variation magnitude histogram
                if (inputParams.ContainsKey("DM_MAGHIST_BINS"))
                {
                    int nBins = (int)inputParams["DM_MAGHIST_BINS"];
                    string histLabel = Set_MagnitudeHistogram_Label(stageIndex_Old, stageIndex_Young, mtxLabel, nBins);

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
                    double cntrRes = ((double[])inputParams["DM_CNTR_BINS"])[0];
                    string contourLabel = Set_ContourCoordinates_Label(stageIndex_Old, stageIndex_Young, mtxLabel, cntrRes);

                    Parallel.Invoke(
                        () => WriteReports(15, 2, 1),
                        () => Generate_contourFigure(pythonPath, dir_MTXwM, dir_dM_PDD, contourLabel, plateLabel)
                        );


                    if (inputParams.ContainsKey("ANG_ROT"))
                    {
                        // Rotated ensemble map
                        Parallel.Invoke(
                            () => WriteReports(15, 3, 1),
                            () => Generate_rotatedCntr_Figure(pythonPath, plateLabel, dir_TMP, dir_MTXwM, dir_dM_PDD)
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
