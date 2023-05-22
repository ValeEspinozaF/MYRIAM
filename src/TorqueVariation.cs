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
    public class TorqueVariation
    {
        /// <summary>
        /// Calculate the torque change, given a transformation matrix and a change in plate motion.
        /// </summary>
        /// <param name="inputParams"><see cref="InputParameters"/> instance with the necessary running parameters (See Remarks)</param>
        /// <param name="dM">Ensemble of torque-variation vector.</param>
        /// <param name="dMm">Average torque variation vector.</param>
        /// <remarks>
        /// Needed input parameters:
        /// <list type="table">
        /// <listheader>
        /// <term>term</term>
        /// <description>description</description>
        /// </listheader>
        /// <item>
        ///     <term>EVy_Path</term>
        ///     <description>String path to a 3-column plain-text file containing an ensemble of sampled Cartesian values 
        ///     of the younger Euler vector, expressed in deg/Myr (degrees per million year). Alternatively, MYRIAM can sample 
        ///     its own ensemble from a singular Euler vector stage, by supplying a 10-column single line containing: 
        ///     [1-2] Euler pole longitude and latitude in degrees, 
        ///     [3] angular velocity magnitude in deg/Myr, 
        ///     [4-9] elements of the covariance matrix associated with the Euler vector ensemble, in rad²/Myr², and 
        ///     [10] size of the ensemble.</description>
        /// </item>
        /// <item>
        ///     <term>EVo_Path</term>
        ///     <description>Same as <see cref="InputParameters.EVy_PATH"/>, but for the older Euler vector. This input is 
        ///     optional, i.e., if omitted MYRIAM will assume that <see cref="InputParameters.EVy_PATH"/> 
        ///     already contains the difference between two Euler vectors at two particular stages.</description>
        /// </item>
        /// <item>
        ///     <term>stageIndex_Old</term>
        ///     <description>First index that serve to identify the "youngest" particular 
        ///     Euler-vector stage used. File labeling prevents overwriting, e.g., when 
        ///     using the same plate but different Euler vectors.</description>
        /// </item>
        /// <item>
        ///     <term>stageIndex_Young</term>
        ///     <description>Second index that serve to identify the ´"oldest" particular 
        ///     Euler-vector stage used.</description>
        /// </item>
        /// <item>
        ///     <term>saveEnsemble</term>
        ///     <description>Boolean that instructs MYRIAM whether to save or not the output torque-variation 
        ///     ensemble in a plain-text file.</description>
        /// </item>
        /// <item>
        ///     <term>dir_MTXwM</term>
        ///     <description>Path to an existing directory where the transformation matrix is stored. The expected file
        ///     name is "MTX_M2w_$.txt", where $ is the matrix label (<see cref="InputParameters.mtxLabel"/>). </description>
        /// </item>
        /// <item>
        ///     <term>dir_dM_PDD</term>
        ///     <description>Path to an existing directory in which all the output files will be stored.</description>
        /// </item>
        /// <item>
        ///     <term>mtxLabel</term>
        ///     <description>String that serves as model label, and helps MYRIAM identify the .</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static void Calculate_dM(
            InputParameters inputParams, 
            out TorqueVector[] dM, out TorqueVector dMm,
            string PATH_MTX_dEV2dM = "", string DIR_dM_PDD = "")
        {

            string EVo_Path = inputParams.EVo_PATH;
            string EVy_Path = inputParams.EVy_PATH;
            int stageIndex_Old = inputParams.STG_IDX_1;
            int stageIndex_Young = inputParams.STG_IDX_2;
            bool saveEnsemble = inputParams.SAVE_ENS;

            string RUN_LABEL =
                $"{inputParams.STG_IDX_1}_" +
                $"{inputParams.STG_IDX_2}_" +
                $"{inputParams.PLT_LABEL}_" +
                $"{inputParams.MODEL_LABEL}"
                ;


            if (PATH_MTX_dEV2dM == "")
                PATH_MTX_dEV2dM = inputParams.MATRIX_PATH;

            if (DIR_dM_PDD == "")
                DIR_dM_PDD = inputParams.DIR_dM_PPD;


            // =========== Calculate Torque variation ==============================

            // Load w2W matrix
            double[,] MTX_w2M = FileReader.File_toArray(PATH_MTX_dEV2dM);


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


            // Dump unused ensembles
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


            // Dump unused ensembles
            dEV = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // Calculate dM's mean vector (Cartesian coordinates) and
            // pole (spherical coordinates), magnitude and covariance
            dMm = EnsembleMean(dM, true);



            // =========== Save Output Text Files ==============================
            

            // Store text file with dM ensemble
            if (saveEnsemble == true)
            {
                string LBL_ENSdM = $"ENSdM_STGs_{RUN_LABEL}.txt";

                Console_Banners.WriteReports(11);
                ManageOutputs.Save_toTXT(dM, DIR_dM_PDD, LBL_ENSdM, format: "#.###E+0");
            }


            // Save dM mean values
            double[] dMmeans = new double[] {
                dMm.X, dMm.Y, dMm.Z,
                dMm.Longitude, dMm.Latitude, dMm.Magnitude,
            };

            dMmeans = dMmeans.Concat(dMm.Covariance.Values()).ToArray();

            string LBL_VECdM = $"VECdM_STGs_{RUN_LABEL}.txt";
            ManageOutputs.Save_toTXT(dMmeans, DIR_dM_PDD, LBL_VECdM, format: "#.###E+0");
        }
    }
}
