using Cartography;
using ComputationalGeometry;
using StructOperations;
using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYRIAM
{
    /// <summary>
    /// Calculates and stores the 3x3 matrix that converts motion change vector into torque-variation vectors.
    /// </summary>
    public class Motion2Torque
    {
        // Earth's radius in km
        static readonly double Re = 6371e3;

        /// <summary>
        /// Matrix that converts motion change vector into torque-variation vectors.
        /// </summary>
        public double[,] MTX_w2M { get; set; }

        /// <summary>
        /// Total area of the plate at its base. 
        /// </summary>
        public double plateArea { get; set; }

        /// <summary>
        /// Array of <see cref = "Coordinate" /> instances with the geographical coordinates of the plate contour, in degrees.
        /// </summary>
        public Coordinate[] cntrArray { get; set; }

        /// <summary>
        /// Array of geographical coordinates of the plate's inner grid points, in degrees. A third column contains the rigidity 
        /// assigned to each grid point, going from [0, 1], where 1 is assigned to a rigid-deemed portions of the plate.
        /// </summary>
        public double[,] gridPoints { get; set; }

        /// <summary>
        /// Rectangular grids for the asthenosphere's depth-average viscosity and Young's modulus.
        /// </summary>
        public AsthenosphereGrids grids { get; set; }


        /// <summary>
        /// This function calculates the 3×3 matrix that link Euler-vector variations to torque-variations, and stores it as a TXT file.
        /// </summary>
        /// <param name="inputParams"><see cref="InputParameters"/> instance with the necessary running parameters (See function overload).</param>
        public void Calculate_dEV2dM_Matrix(
            InputParameters inputParams)
        {
            // ============== Parameters ===============================================

            string contourPath = inputParams.CTR_PATH;
            double muM = inputParams.muM;
            double muA = inputParams.muA;
            double fHA = inputParams.FRACTION_HA;
            double HL = inputParams.HL_km * 1e3;
            double stepDeg = inputParams.GRID_RES;
            CartoLimits REGION_muA_LV = inputParams.REGION_muA_LV;
            double defLength = inputParams.DEF_DISTANCE_km * 1e3;
            string plateLabel = inputParams.PLT_LABEL;
            string DIR_MTX_dEV2dM = inputParams.DIR_MTX_w2M;
            string modelName = inputParams.MODEL_LABEL;


            // ============== dEV - dM operators =======================================

            Console_Banners.WriteReports(7);
            Calculate_dEV2dM_Matrix(
                contourPath, muM, muA, HL, fHA, stepDeg, REGION_muA_LV, defLength
                );


            // ============== Save TXT files ===========================================
            Console_Banners.WriteReports(8);
            Save_w2M_Outputs(HL, plateLabel, modelName, DIR_MTX_dEV2dM, out string matrixPath);
            inputParams.Add("MATRIX_PATH", matrixPath);

        }


        /// <summary>
        /// This function calculates the 3×3 matrix that link Euler-vector variations to torque-variations.
        /// </summary>
        /// <param name="contourPath">String path to a plain-text file containing the plate contour coordinates as two 
        /// columns: [1] longitude and [2] latitude values, both expressed in degrees.</param>
        /// <param name="muM">Average value of the lower part of the upper mantle viscosity, expressed in Pa · s.</param>
        /// <param name="muA">Average value of the asthenosphere viscosity, expressed in Pa · s.</param>
        /// <param name="HL">Value of the lithosphere thickness (i.e., depth of the lithosphere-asthenosphere boundary), expressed in meters.</param>
        /// <param name="fHA">Value between 0 and 1 that sets the vertical fraction of the region utilized to define the depth-average viscosity 
        /// of the asthenosphere using the model PM_v2_2012 by Priestley and McKenzie, 2013.</param>
        /// <param name="stepDeg">Value for the grid resolution of the plate's base, expressed in degrees.</param>
        /// <param name="REGION_muA_LV"><see cref = "CartoLimits" /> instance with the geographical limits(in degrees) within which<c>muA</c> is average.</param>
        /// <param name="defLength">Value used to set a deformation buffer width (in meters) across the plate boundary. The boundary region has a 
        /// linearly-decreased Euler vector magnitude, acting as a damped rigidity, as yielding an overall smaller torque-variation estimate.</param>
        public void Calculate_dEV2dM_Matrix(
            string contourPath,
            double muM,
            double muA,
            double HL,
            double fHA,
            double stepDeg,
            CartoLimits REGION_muA_LV,
            double defLength
            )
        {


            // ============== Parameters ===============================================

            double stepRad = stepDeg * (Math.PI / 180);
            double Rt = Re - HL;




            // ============== Plate basal grid =============================

            Console_Banners.WriteReports(3);

            // Upload plate contour spherical coordinates
            List<Coordinate[]> cntrArrays = FileReader.File_CoordinatesArray(contourPath);
            this.cntrArray = ManageCoordinates.ConcatenateCoords(cntrArrays);


            if (!REGION_muA_LV.IsEmpty() && !REGION_muA_LV.Encloses(cntrArray))
                throw new InputErrorException(
                    "Error in REGION_muA_LV. " +
                    "Region does not contain the whole plate contour. " +
                    "Fix this by expanding the limits set in parameter REGION_muA_LV.");


            // Create grid of coordinates within the plate boundary limits
            Coordinate[] gridPntsDeg = ManageCoordinates.Grid_InPolygon(cntrArrays, stepDeg);


            // Turn filtered matrix coordinates to spherical radians and Cartesian
            Coordinate[] gridPntsRad = Coordinate.ToRadians(gridPntsDeg);
            Vector[] gridPntsCart = Coordinate.ToCartesian(gridPntsDeg, Rt);




            // ============== Plate basal area =============================

            // Area partitioning 
            double[] dArea = new double[gridPntsRad.Length];
            for (int i = 0; i < gridPntsRad.Length; i++)
            {
                dArea[i] = Math.Pow(Rt, 2)
                    * (-Math.Cos(gridPntsRad[i].Lat + (stepRad / 2))
                       + Math.Cos(gridPntsRad[i].Lat - (stepRad / 2)))
                    * stepRad;
            }

            // Plate area
            this.plateArea = dArea.Sum();



            // ======= Deformation buffer (if DEF_DISTANCE_km is given) =======

            // Default value is 1
            double[] fDef = ArrayManagement.Repeat1D(1.0, dArea.Length);
            this.gridPoints = Coordinate.ToArray(gridPntsDeg, fDef);

            if (defLength != 0)
            {
                // Densify plate contour 
                cntrArray = PolygonGeometry.DensifyPolygon_toDistance(cntrArray, defLength / 2);

                fDef = DeformationArea.DeformationGridScore(gridPntsDeg, cntrArray, defLength);

                // Apply deformation fraction buffer to grid (in Area grid for convenience)
                dArea = ArrayOperations.ArrayProduct(dArea, fDef);
            }



            // ======= Asthenosphere viscosity / thickness interpolation ======
            this.grids = new();
            this.grids.Map_muA_AverageGrid(muM, muA, HL, fHA, REGION_muA_LV);


            Console_Banners.WriteReports(6);
            double[] muA_gridPnts = GridInterpolation.Interpolation2D(
                grids.muA_mlon, grids.muA_mlat, grids.muA_mdat, gridPntsDeg, "linear");


            double[] muAfHA = ArrayOperations.ArrayDivide(muA_gridPnts, grids.HA);



            // ============== dEV - dM operators =======================================

            this.MTX_w2M = build_MTX_w2M(muAfHA, dArea, gridPntsCart);
        }


        public void Save_w2M_Outputs(double HL, string plateLabel, string modelName, string dirPath, out string matrixPath)
        {
            // --- Save plate Area TXT file ---
            string filenameArea = ManageOutputs.Set_Area_FileName(plateLabel, HL);
            ManageOutputs.Save_toTXT(plateArea, dirPath, filenameArea);


            // --- Save plate boundary coordinates TXT file ---
            string filenameBoundary = ManageOutputs.Set_Boundary_FileName(plateLabel);
            ManageOutputs.Save_toTXT(cntrArray, dirPath, filenameBoundary);


            // --- Save grid coordinates TXT file ---
            string filenameInBoundary = ManageOutputs.Set_BoundaryIn_FileName(plateLabel);
            ManageOutputs.Save_toTXT(gridPoints, dirPath, filenameInBoundary, "F2");


            // --- Save grid values for muA (Asthenosphere's Viscosity), YM (Young's Module) and TXT file ---
            grids.Save_AverageGrids(modelName, dirPath);


            // --- Save m2M TXT file ---
            string FILENAME_w2M = ManageOutputs.Set_Matrix_w2M_FileName(plateLabel, modelName);
            matrixPath = Path.Combine(dirPath, FILENAME_w2M);
            ManageOutputs.Save_toTXT(MTX_w2M, dirPath, FILENAME_w2M);
        }

        private static double[,] build_MTX_w2M(double[] muAfHA, double[] dA, Vector[] cartArray)
        {
            // --- Matrix MTX_w2M (for MTX_w2M * w = M) ---

            // Replace negative values with zeros
            muAfHA = muAfHA.Select(x => x < 0 ? 0 : x).ToArray();


            // Set empty 3x3 matrix
            double[,] Matrix = new double[3, 3];


            // Calculate squared coordinates beforehand
            double[] Xsqrd = (from vector in cartArray select Math.Pow(vector.X, 2)).ToArray();
            double[] Ysqrd = (from vector in cartArray select Math.Pow(vector.Y, 2)).ToArray();
            double[] Zsqrd = (from vector in cartArray select Math.Pow(vector.Z, 2)).ToArray();


            for (int j = 0; j < muAfHA.Length; j++)
            {
                Matrix[0, 0] += muAfHA[j] * dA[j] * (Ysqrd[j] + Zsqrd[j]);
                Matrix[0, 1] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Y);
                Matrix[0, 2] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Z);
                Matrix[1, 0] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Y);
                Matrix[1, 1] += muAfHA[j] * dA[j] * (Xsqrd[j] + Zsqrd[j]);
                Matrix[1, 2] += muAfHA[j] * dA[j] * (cartArray[j].Y * cartArray[j].Z);
                Matrix[2, 0] += muAfHA[j] * dA[j] * (cartArray[j].X * cartArray[j].Z);
                Matrix[2, 1] += muAfHA[j] * dA[j] * (cartArray[j].Y * cartArray[j].Z);
                Matrix[2, 2] += muAfHA[j] * dA[j] * (Xsqrd[j] + Ysqrd[j]);
            }

            // Multiply times -1
            Matrix[0, 1] = -1.0 * Matrix[0, 1];
            Matrix[0, 2] = -1.0 * Matrix[0, 2];
            Matrix[1, 0] = -1.0 * Matrix[1, 0];
            Matrix[1, 2] = -1.0 * Matrix[1, 2];
            Matrix[2, 0] = -1.0 * Matrix[2, 0];
            Matrix[2, 1] = -1.0 * Matrix[2, 1];

            return Matrix;
        }
    }
}
