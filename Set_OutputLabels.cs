using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;


namespace MYRIAM
{
    class Set_OutputLabels
    {
        public static string Set_Model_Label(double fHA, double muM, double muA, double HL, CoordsLimits REGION_muA_LV, double defLength)
        {
            string SV_MODEL_NAME = "";

            // Set up map extention label
            string AvExtension_Lbl = Set_MapExtension_Label(REGION_muA_LV);


            // Set up model name label
            if (REGION_muA_LV.IsEmpty())
                SV_MODEL_NAME =
                    "A" + (muA / 1e19) +
                    "_M" + (muM / 1e20) +
                    "_HL" + Math.Ceiling(HL) +
                    "_NoLV" +
                    "_D" + (defLength / 1e2);

            else if (fHA == 1)
                SV_MODEL_NAME = 
                    "A" + (muA / 1e19) + 
                    "_M" + (muM / 1e20) +  
                    "_HL" + Math.Ceiling(HL) + 
                    "_"  + AvExtension_Lbl + "_fHA1p0" +
                    "_D" + (defLength / 1e2);

            else if (fHA < 1)
                SV_MODEL_NAME = 
                    "A" + (muA / 1e19) + 
                    "_M" + (muM / 1e20) + 
                    "_HL" + Math.Ceiling(HL) + 
                    "_" + AvExtension_Lbl + "_fHA0p" + Math.Ceiling(fHA*10) +
                    "_D" + (defLength / 1e2);


            return SV_MODEL_NAME;
        }


        public static string Set_MapExtension_Label(CoordsLimits avExt)
        {
            string SQUARE_lbl = "";


            // Default global extension array
            var glbExt = new CoordsLimits().SetGlobal();


            // Compare map extension with global extension
            if (avExt.IsEqual(glbExt))
            {
                // Set label as global
                SQUARE_lbl = "GLBL";
            }

            else
            {
                // Cardinal coordinate labels
                var cardLbls = new string[4]
                {
                    (avExt.LonMin < 0) ? "W" : "E",
                    (avExt.LonMax < 0) ? "W" : "E",
                    (avExt.LatMin < 0) ? "S" : "N",
                    (avExt.LatMax < 0) ? "S" : "N"
                };                

                for (int i = 0; i < 4; i++)
                    SQUARE_lbl += Math.Abs(avExt.ToArray()[i]) + cardLbls[i];

            }
            return SQUARE_lbl;
        }


        public static string Set_Matrix_Label(string plateLabel, string interpMethod, string modelName)
        {
            string MATRIX_LABEL = plateLabel + "_" + modelName;

            if (interpMethod == "linear")
                MATRIX_LABEL += "_iLINR";

            else if (interpMethod == "nearest")
                MATRIX_LABEL += "_iNRST";

            return MATRIX_LABEL;
        }


        public static string Set_Matrix_w2M_FileName(string plateLabel, string modelName, string interpMethod)
        {
            string interpStr = interpMethod == "linear" ? "iLINR" : "iNRST";

            string LBL_w2M = 
                $"MTX_w2M" +
                $"_{plateLabel}" +
                $"_{modelName}" +
                $"_{interpStr}.txt";

            return LBL_w2M;
        }


        public static string Set_Area_FileName(string plateLabel, double HL)
        {
            return $"AREA_{plateLabel}_HL{(HL/1000).ToString().Replace(",", "p")}km.txt";
        }

        public static string Set_Boundary_FileName(string plateLabel)
        {
            return $"BDR_{plateLabel}.txt";
        }


        public static string Set_BoundaryIn_FileName(string plateLabel)
        {
            return $"BDRin_{plateLabel}.txt";
        }


        public static void Set_GridValues_FileNames(string modelName, 
            out string GRID_LON_FILENAME, out string GRID_LAT_FILENAME, 
            out string GRID_MuA_FILENAME, out string GRID_YM_FILENAME,
            out string GRID_MT_FILENAME)
        {
            GRID_LON_FILENAME = "GRID_LON_" + modelName + ".txt";
            GRID_LAT_FILENAME = "GRID_LAT_" + modelName + ".txt";
            GRID_MuA_FILENAME = "GRID_MuA_" + modelName + ".txt";
            GRID_YM_FILENAME = "GRID_YM_" + modelName + ".txt";
            GRID_MT_FILENAME = "GRID_MT_" + modelName + ".txt";
        }

        public static string Set_ContourCoordinates_FileName(double percentageLevel, double STG_OLD, double STG_YOUNG, string MATRIX_LABEL, double gstep)
        {
            string CNTR_PDD_LBL = Set_ContourCoordinates_Label(STG_OLD, STG_YOUNG, MATRIX_LABEL, gstep);

            if (percentageLevel.ToString().Contains(','))
                CNTR_PDD_LBL = $"CNTR" + string.Format(percentageLevel.ToString()).Replace(",", "p") + CNTR_PDD_LBL + ".txt";
            else
                CNTR_PDD_LBL = $"CNTR" + string.Format(percentageLevel.ToString()).Replace(".", "p") + CNTR_PDD_LBL + ".txt";

            return CNTR_PDD_LBL;
        }


        public static string Set_ContourCoordinates_Label(double STG_OLD, double STG_YOUNG, string matrixLabel, double gStep)
        {
            string CNTR_PDD_LBL = $"_STGs_{STG_OLD}_{STG_YOUNG}_{matrixLabel}_r";

            if (gStep.ToString().Contains(','))
                CNTR_PDD_LBL += string.Format(gStep.ToString()).Replace(",", "p");
            else
                CNTR_PDD_LBL += string.Format(gStep.ToString()).Replace(".", "p");

            return CNTR_PDD_LBL;
        }

        public static string Set_MagnitudeHistogram_Label(double STG_OLD, double STG_YOUNG, string MATRIX_LABEL, int nBins)
        {
            return $"STGs_{STG_OLD}_{STG_YOUNG}_{MATRIX_LABEL}_{nBins}";
        }

        public static string Set_MagnitudeHistogram_FileName(double STG_OLD, double STG_YOUNG, string MATRIX_LABEL, int nBins)
        {
            string label = Set_MagnitudeHistogram_Label(STG_OLD, STG_YOUNG, MATRIX_LABEL, nBins);
            return $"MAGHIST_{label}.txt";
        }
    }
}
