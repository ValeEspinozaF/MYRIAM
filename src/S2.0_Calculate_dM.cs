using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EnsembleAnalysis.Ensemble_Statistics;
using DataStructures;


namespace MYRIAM
{
    internal partial class MainFunctions
    {
        public static void Calculate_dM(
            InputParameters inputParams, out TorqueVector[] dM, out TorqueVector dMm)
        {
            string EVo_Path = inputParams.EVo_PATH;
            string EVy_Path = inputParams.EVy_PATH;
            int stageIndex_Old = inputParams.STG_IDX_1;
            int stageIndex_Young = inputParams.STG_IDX_2;
            bool saveEnsemble = inputParams.SAVE_ENS;

            string dir_MTXwM = inputParams.DIR_MTX_w2M;
            string dir_dM_PDD = inputParams.DIR_dM_PPD;
            string mtxLabel = inputParams.MTX_LABEL;


            // =========== Calculate Torque variation ==============================

            // Load w2W matrix
            string w2M_path = Path.Combine(dir_MTXwM, $"MTX_w2M_{mtxLabel}.txt");
            double[,] MTX_w2M = FileReader.File_toArray(w2M_path);


            // Load Euler vectors
            EulerVector[] EVo = new EulerVector[0];
            EulerVector[] EVy = new EulerVector[0];

            Parallel.Invoke(
                () => Console_Banners.WriteReports(9),
                () => Load_EulerVector.Load_EulerVectors(
                        EVo_Path, EVy_Path,
                        out EVo, out EVy)
                );


            // Ensemble length
            if (EVo.Length != EVy.Length)
                throw new InputErrorException(
                    "Error! " +
                    "Old and young ensemble are not the same length."
                    );


            // Set Euler Vector ensemble change from deg/Myr to in rad/s
            double unitsTransformer = Math.PI / 180 / (1e6 * 365 * 24 * 60 * 60);
            EulerVector[]? dEV = EulerVector.VectorMultiply( EulerVector.VectorSubstract(EVy, EVo), unitsTransformer );


            // Dump un-used ensembles
            EVo = new EulerVector[0];
            EVy = new EulerVector[0];
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Calculate torque variation (dM) by applying the product of EV-change (dEV) and MTX_w2M
            Console_Banners.WriteReports(10);

            dM = new TorqueVector[dEV.Length];
            for (int i = 0; i < dEV.Length; i++)
            {
                dM[i] = new TorqueVector
                {
                    X = MTX_w2M[0, 0] * dEV[i].X + MTX_w2M[0, 1] * dEV[i].Y + MTX_w2M[0, 2] * dEV[i].Z,
                    Y = MTX_w2M[1, 0] * dEV[i].X + MTX_w2M[1, 1] * dEV[i].Y + MTX_w2M[1, 2] * dEV[i].Z,
                    Z = MTX_w2M[2, 0] * dEV[i].X + MTX_w2M[2, 1] * dEV[i].Y + MTX_w2M[2, 2] * dEV[i].Z
                };
            }


            // Dump un-used ensembles
            dEV = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Calculate dM's mean vector (cartesian coordinates) and
            // pole (spherical coordinates), magnitude and covariance
            dMm = EnsembleMean(dM, true);



            // =========== Save Output Text Files ==============================

            

            // Store text file with dM ensemble
            if (saveEnsemble == true)
            {
                string LBL_ENSdM = $"ENSdM_STGs_{stageIndex_Old}_{stageIndex_Young}_{mtxLabel}.txt";

                Console_Banners.WriteReports(11);
                ManageOutputs.Save_toTXT(dM, dir_dM_PDD, LBL_ENSdM, format: "#.###E+0");
            }


            // Save dM mean values
            double[] dMmeans = new double[] {
                dMm.X, dMm.Y, dMm.Z,
                dMm.Longitude, dMm.Latitude, dMm.Magnitude,
            };

            dMmeans = dMmeans.Concat(dMm.Covariance.Values()).ToArray();

            string LBL_VECdM = $"VECdM_STGs_{stageIndex_Old}_{stageIndex_Young}_{mtxLabel}.txt";
            ManageOutputs.Save_toTXT(dMmeans, dir_dM_PDD, LBL_VECdM, format: "#.###E+0");
        }
    }
}
