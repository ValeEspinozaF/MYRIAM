using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StructOperations.ArrayManagement;
using static StructOperations.VectorOperations;
using static EnsembleAnalysis.Ensemble_Statistics;
using CartographicCoordinates;
using StructOperations;
using DataStructures;
using EnsembleAnalysis;



namespace MYRIAM
{
    internal class FNCT_EVdM
    {
        public static void EV_to_dM(int stageIndex_Old, int stageIndex_Young,
                                    string EVo_Path, string EVy_Path, string dir_MTXwM, string dir_dM_PDD,
                                    string mtxLabel, bool saveEnsemble, 
                                    out vectorCart[] dM, out VectorDegCov dMvector)
        {

            // =========== Calculate Torque variation ==============================

            // Load w2W matrix
            string w2M_path = Path.Combine(dir_MTXwM, $"MTX_w2M_{mtxLabel}.txt");
            double[,] MTX_w2M = FileReader.File_toArray(w2M_path);


            // Load Euler vectors
            vectorCart[] EVo = new vectorCart[0];
            vectorCart[] EVy = new vectorCart[0];

            Parallel.Invoke(
                () => Console_Banners.WriteReports(9),
                () => Manage_InputParams.Load_EV_Ensembles(
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
            vectorCart[]? dEV = VectorMultiply( VectorSubstract(EVy, EVo), unitsTransformer );


            // Dump un-used ensembles
            EVo = new vectorCart[0];
            EVy = new vectorCart[0];
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Calculate torque variation (dM) by applying the product of EV-change (dEV) and MTX_w2M
            Console_Banners.WriteReports(10);
            dM = VectorProduct(dEV, MTX_w2M);


            // Dump un-used ensembles
            dEV = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Calculate dM's mean pole (spherical coordinates), magnitude and covariance
            dMvector = Ensemble_ToVectorCov(dM);


            // Calculate dM's mean vector (cartesian coordinates)
            vectorCart dMCart_mean = EnsembleMean(dM);



            // =========== Save Output Text Files ==============================

            

            // Store text file with dM ensemble
            if (saveEnsemble == true)
            {
                string LBL_ENSdM = $"ENSdM_STGs_{stageIndex_Old}_{stageIndex_Young}_{mtxLabel}.txt";

                Console_Banners.WriteReports(11);
                ManageOutputs.Save_toTXT(dM, dir_dM_PDD, LBL_ENSdM, format: "#.###E+0");
            }


            // Save dM mean values
            double[] dMmeans = dMCart_mean.Values().Concat(dMvector.Values()).ToArray();

            string LBL_VECdM = $"VECdM_STGs_{stageIndex_Old}_{stageIndex_Young}_{mtxLabel}.txt";
            ManageOutputs.Save_toTXT(dMmeans, dir_dM_PDD, LBL_VECdM, format: "#.###E+0");
        }
    }
}
