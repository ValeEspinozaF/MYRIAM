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
using DataStructures;


namespace MYRIAM
{
    class EntryPoint
    {
        static public void Main(string[] args)
        {
            // --- Opening Console Banner ---
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
                    "The software call must be followed by the path to the inputs text file.");
            }

            // Set run parameters
            InputParameters inputParams = Manage_InputParams.Set_InputParams(inputFilePath);


            // Main inputs banner
            InputSummaryBanner(inputParams);



            // ========== Set output directories and file labels ==================

            // Create output directories
            WriteReports(2);
            Create_OutputDirs(inputParams);


            // Set up model and matrix labels
            inputParams.Add("MODEL_LABEL", Set_Model_Label(inputParams));
            inputParams.Add("RUN_LABEL", Set_Run_Label(inputParams));




            // ================== First Main Equation ==============================
            // Euler vector(w) to torque(M) with lateral variations of asthenosphere
            // viscosity(muA)

            Motion2Torque m2W = new();
            m2W.Calculate_dEV2dM_Matrix(inputParams);



            // ================== Second Main Equation =============================
            // Calculates the torque-variation (dM) associated with an Euler-vector
            // change(dEV) of a given tectonic plate

            TorqueVariation.Calculate_dM(
                inputParams, out TorqueVector[] dM, out TorqueVector dMvector
                );


            OutputSummaryBanner(dMvector);



            // ================== Optional dM Plotting calculations ================
            // Calculate the distributions of magnitude (histogram) and
            // pole (confidence contour), and output them via TXT files.
            
            if (inputParams.ContainsKey("DM_MAGHIST_BINS"))
            {
                WriteReports(12, 1);

                Torque_EnsembleStatistics.Magnitude_Histogram(
                    inputParams, dM);
                
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
                double[,] ROT_MTX = Matrix.Build_RotationMatrix(ANG_R[0], ANG_R[1], ANG_R[2]);


                // === Calculate Contours ===
                Torque_EnsembleStatistics.Pole_Contours(
                    inputParams, dM, ROT_MTX, DM_CNTR_BINS, DM_CNTR_PERCENT);

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


                // Generate Asthenosphere-Viscosity, Young's-Modulus and Maxwell-time maps
                Parallel.Invoke(
                        () => WriteReports(15, 0),
                        () => Generate_gridFigures(inputParams)
                        );


                // Generate Torque variation magnitude histogram
                if (inputParams.ContainsKey("DM_MAGHIST_BINS"))
                {
                    Parallel.Invoke(
                        () => WriteReports(15, 1, 1),
                        () => Generate_histogramFigure(inputParams)
                        );
                }
                else
                    WriteReports(15, 1, 0);


                // Generate Torque variation pole distribution maps
                if (inputParams.ContainsKey("DM_CNTR_BINS"))
                {
                    Parallel.Invoke(
                        () => WriteReports(15, 2, 1),
                        () => Generate_contourFigure(inputParams)
                        );


                    if (inputParams.ContainsKey("ANG_ROT"))
                    {
                        // Rotated ensemble map
                        Parallel.Invoke(
                            () => WriteReports(15, 3, 1),
                            () => Generate_rotatedCntr_Figure(inputParams)
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

            Delete_TMPs(inputParams.DIR_TMP);
            WriteReports(16);
        }
    }
}
