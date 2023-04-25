using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using StructOperations;
using DataStructures;
using ComputationalGeometry;
using static CartographicCoordinates.ManageCoordinates;
using static MYRIAM.ManageOutputs;
using Cartography;

namespace MYRIAM
{
    internal class Calculate_dEV2dM_Matrix
    {
        // Earth's radius in km
        static readonly double Re = 6371e3;

        /// <summary>
        /// This function calculates the matrix that link Euler-vector variations 
        /// to torque-variations.
        /// </summary>
        /// <param name="dir_MTXwM"></param>
        /// <param name="modelName">String with the name of the current model.</param>
        public static void w2M_MTX_CorLV_muA(InputParameters inputParams, string dir_MTXwM, string modelName)
        {
            string contourPath = inputParams.CTR_PATH;
            double muM = inputParams.muM;
            double muA = inputParams.muA;
            double HL = inputParams.HL * 1e3;
            double fHA = inputParams.FRACTION_HA;
            double stepDeg = inputParams.GRID_RES;
            CartoLimits REGION_muA_LV = inputParams.REGION_muA_LV;
            string interpMethod = inputParams.INTERP_MTD;
            double defLength = inputParams.DEF_DISTANCE * 1e3;
            string plateLabel = inputParams.PLT_LABEL;


            double stepRad = stepDeg * (Math.PI / 180);
            double Rt = Re - HL;


            // ============== Plate basal grid =============================

            Console_Banners.WriteReports(3);

            // Upload plate contour spherical coordinates
            List<Coordinate[]> cntrArrays = FileReader.File_CoordinatesArray(contourPath);
            Coordinate[] cntrArray = ConcatenateCoords(cntrArrays);


            if (!REGION_muA_LV.IsEmpty() && !REGION_muA_LV.Encloses(cntrArray))
                throw new InputErrorException(
                    "Error in REGION_muA_LV. " +
                    "Region does not contain the whole plate contour. " +
                    "Fix this by expanding the limits set in parameter REGION_muA_LV.");


            // Create grid of coordinates within the plate boundary limits
            Coordinate[] gridPntsDeg = Grid_InPolygon(cntrArrays, stepDeg);


            // Turn filtered matrix coordinates to spherical radians and cartesian
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
            double plateArea = dArea.Sum();



            // ======= Deformation buffer (if DEF_DISTANCE is given) =======

            // Default value is 1
            double[] fDef = ArrayManagement.Repeat1D(1.0, dArea.Length);

            if (defLength != 0)
            {
                // Densify plate contour 
                cntrArray = PolygonGeometry.DensifyPolygon_toDistance(cntrArray, defLength / 2);

                fDef = DeformationArea.DeformationGridScore(gridPntsDeg, cntrArray, defLength);

                // Apply deformation fraction buffer to grid (in Area grid for convenience)
                dArea = ArrayOperations.ArrayProduct(dArea, fDef);
            }



            // ======= Asthenosphere viscosity / thickness interpolation ======

            FNCT_muA_MAPPER.muA_MAPPER(
                muM, muA, HL, REGION_muA_LV, fHA, 
                out double HA, out double[,] muA_mlon, 
                out double[,] muA_mlat, out double[,] muA_mdat, 
                out double[,] YM_mdat, out double[,] MT_mdat);

            
            double[] muA_gridPnts = GridInterpolation.Interpolation2D( 
                muA_mlon, muA_mlat, muA_mdat, gridPntsDeg, interpMethod);


            double[] muAfHA = ArrayOperations.ArrayDivide(muA_gridPnts, HA);



            // ============== dEV - dM operators =======================================


            Console_Banners.WriteReports(7);
            double[,] MTX_w2M = build_MTX_w2M(muAfHA, dArea, gridPntsCart);



            // ============== Save TXT files ===========================================
            Console_Banners.WriteReports(8);

            // --- Save m2M TXT file ---
            string FILENAME_w2M = Set_Matrix_w2M_FileName(plateLabel, modelName, interpMethod);
            Save_toTXT(MTX_w2M, dir_MTXwM, FILENAME_w2M);


            // --- Save plate Area TXT file ---
            string filenameArea = Set_Area_FileName(plateLabel, HL);
            Save_toTXT(plateArea, dir_MTXwM, filenameArea);


            // --- Save plate boundary coordinates TXT file ---
            string filenameBoundary = Set_Boundary_FileName(plateLabel);
            Save_toTXT(cntrArray, dir_MTXwM, filenameBoundary);


            // --- Save grid coordinates TXT file ---
            string filenameInBoundary = Set_BoundaryIn_FileName(plateLabel);
            double[,] gridPoints = Coordinate.ToArray(gridPntsDeg, fDef);
            Save_toTXT(gridPoints, dir_MTXwM, filenameInBoundary, "F2");


            // --- Save grid values for muA (Asthenospheres Viscosity), YM (Youngs Modeule) and TXT file ---
            Set_GridValues_FileNames(modelName, 
                out string GRID_LON_FILENAME, out string GRID_LAT_FILENAME, 
                out string GRID_MuA_FILENAME, out string GRID_YM_FILENAME,
                out string GRID_MT_FILENAME);

            Save_toTXT(muA_mlon, dir_MTXwM, GRID_LON_FILENAME, "0.0");
            Save_toTXT(muA_mlat, dir_MTXwM, GRID_LAT_FILENAME, "0.0");
            Save_toTXT(muA_mdat, dir_MTXwM, GRID_MuA_FILENAME);
            Save_toTXT(YM_mdat, dir_MTXwM, GRID_YM_FILENAME);
            Save_toTXT(MT_mdat, dir_MTXwM, GRID_MT_FILENAME);
        }

        private static double[,] build_MTX_w2M(double[] muAfHA, double[] dA, Vector[] cartArray)
        {
            // --- Matrix MTX_w2M (for MTX_w2M * w = M) ---

            // Replate negative values with zeros
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
