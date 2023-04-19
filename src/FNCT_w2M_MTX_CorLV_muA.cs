using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartographicCoordinates;
using StructOperations;
using DataStructures;
using ComputationalGeometry;
using static CartographicCoordinates.TransformSystem;
using static CartographicCoordinates.ManageCoordinates;
using static MYRIAM.ManageOutputs;
using static MYRIAM.Set_OutputLabels;
using System.Xml.Schema;

namespace MYRIAM
{
    internal class FNCT_w2M_MTX_CorLV_muA
    {
        // Earth's radius in km
        static readonly double Re = 6371e3;

        /// <summary>
        /// This function calculates the matrix that link Euler-vector variations 
        /// to torque-variations.
        /// </summary>
        /// <param name="dir_MTXwM"></param>
        /// <param name="contourPath"></param>
        /// <param name="stageIndex_Old"></param>
        /// <param name="stageIndex_Young"></param>
        /// <param name="muM"></param>
        /// <param name="muA"></param>
        /// <param name="HL">Average thickness of the lithosphere, in kilometers.</param>
        /// <param name="fHA">Vertical fraction of the asthenospheric channel to use for calculations.</param>
        /// <param name="stepDeg">Spacial spacing for the grid sampling, in degrees.</param>
        /// <param name="REGION_muA_LV"></param>
        /// <param name="interpMethod"></param>
        /// <param name="defLength"></param>
        /// <param name="plateLabel"></param>
        /// <param name="modelName">String with the name of the current model.</param>
        public static void w2M_MTX_CorLV_muA( 
            string dir_MTXwM, string contourPath, 
            double muM, double muA, double HL, double fHA, 
            double stepDeg, CoordsLimits REGION_muA_LV, string interpMethod, 
            double defLength, string plateLabel, string modelName)
        {

            // ============== Filling plate basal area =============================


            // === Points inside plate area ===
            Console_Banners.WriteReports(3);

            // Upload plate contour spherical coordinates
            List<Coord[]> cntrArrays = FileReader.File_CoordinatesArray(contourPath);
            Coord[] cntrArray = ConcatenateCoords(cntrArrays);

            if (!REGION_muA_LV.IsEmpty() && !REGION_muA_LV.Enclose(cntrArray))
                throw new InputErrorException(
                    "Error in REGION_muA_LV. " +
                    "Region does not contain the whole plate contour. " +
                    "Fix this by expanding the limits set in parameter REGION_muA_LV.");


            // Create grid of coordinates within the plate boundary limits
            // (may be multiple), with stepDeg as spacing
            Coord[] gridPntsDeg = cntrArrays.
                SelectMany(coordArray => Grid_InPolygon(coordArray, stepDeg)).
                ToArray();


            // Eliminate grid duplicates (same latitude, one with X=180, the other X=-180)
            gridPntsDeg = gridPntsDeg.
                Where(x => !(x.X == -180 && gridPntsDeg.Where(p => p.X == 180).Select(p => p.Y).Contains(x.Y))).
                ToArray();


            // Turn filtered matrix coordinates from radians to degrees
            Coord[] gridPntsRad = DegToRad(gridPntsDeg);

            

            // === Plate area ===

            double stepRad = ToRadians(stepDeg);
            double[] Rt = ArrayManagement.Repeat1D(Re - HL, gridPntsDeg.Length);
            double[] dArea = new double[gridPntsRad.Length];
            

            // Area partitioning 
            for (int i = 0; i < gridPntsRad.Length; i++)
            {
                dArea[i] = Math.Pow(Rt[i], 2) 
                    * (-Math.Cos(gridPntsRad[i].Y + (stepRad / 2)) 
                       + Math.Cos(gridPntsRad[i].Y - (stepRad / 2))) 
                    * stepRad;
            }


            // Turn inplate coordinates from radians to cartesian
            Coord[] gridPntsCart = RadToCart(gridPntsRad, Rt);


            // Plate area
            double plateArea = dArea.Sum();



            // === Deformation buffer (if defLength is given) ===

            // default value is 1
            double[] fDef = ArrayManagement.Repeat1D(1.0, dArea.Length);

            if (defLength != 0)
            {
                // Densify plate contour 
                cntrArray = DensifyPolygon.DensifyPolygon_toDistance(cntrArray, defLength / 2);

                fDef = DeformationArea.DeformationGridScore(gridPntsDeg, cntrArray, defLength);

                // Apply deformation fraction buffer to grid (in Area grid for convenience)
                dArea = ArrayOperations.ArrayProduct(dArea, fDef);
            }



            // ========== Asthenosphere viscosity / thickness interpolation ===========

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
            double[,] MTX_w2M = Operator_dEVdM.Build_MTX_w2M(muAfHA, dArea, gridPntsCart);



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
            double[,] gridPoints = ToArray(gridPntsDeg, fDef);
            Save_toTXT(gridPoints, dir_MTXwM, filenameInBoundary, "F2");


            // --- Save grid values for muA (Asthenospheres Viscosity), YM (Youngs Modeule) and   TXT file ---
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
    }
}
