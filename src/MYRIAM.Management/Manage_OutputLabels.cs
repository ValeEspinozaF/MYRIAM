using DataStructures;
using Cartography;


namespace MYRIAM
{
    partial class ManageOutputs
    {
        public static string Set_Model_Label(InputParameters inputParams)
        {
            double fHA = inputParams.FRACTION_HA;
            double muM = inputParams.muM;
            double muA = inputParams.muA;
            double HL = inputParams.HL_km;
            CartoLimits REGION_muA_LV = inputParams.REGION_muA_LV;
            double defLength = inputParams.DEF_DISTANCE_km;


            string SV_MODEL_NAME = "";

            // Set up map extension label
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

        public static string Set_Run_Label(InputParameters inputParams)
        {
            return $"STGs_{inputParams.STG_IDX_1}_{inputParams.STG_IDX_2}_{inputParams.PLT_LABEL}_{inputParams.MODEL_LABEL}";
        }


        public static string Set_MapExtension_Label(CartoLimits setExt)
        {
            string SQUARE_lbl = "";


            // Default global extension array
            var globalExt = new CartoLimits().SetGlobal();


            // Compare map extension with global extension
            if (setExt.Equals(globalExt))
            {
                // Set label as global
                SQUARE_lbl = "GLBL";
            }

            else
            {
                // Cardinal coordinate labels
                var cardLbls = new string[4]
                {
                    (setExt.LonMin < 0) ? "W" : "E",
                    (setExt.LonMax < 0) ? "W" : "E",
                    (setExt.LatMin < 0) ? "S" : "N",
                    (setExt.LatMax < 0) ? "S" : "N"
                };                

                for (int i = 0; i < 4; i++)
                    SQUARE_lbl += Math.Abs(setExt.ToArray()[i]) + cardLbls[i];

            }
            return SQUARE_lbl;
        }


        public static string Set_Matrix_w2M_FileName(string plateLabel, string modelName, int stg_young, int stg_old)
        {
            string LBL_w2M = 
                $"MTX_w2M" +
                $"_{stg_young}_{stg_old}" +
                $"_{plateLabel}" +
                $"_{modelName}.txt";

            return LBL_w2M;
        }


        public static string Set_Area_FileName(string plateLabel, double HL, int stg_young, int stg_old)
        {
            return $"AREA_{plateLabel}_{stg_young}_{stg_old}_HL{(HL/1000).ToString().Replace(",", "p")}km.txt";
        }

        public static string Set_Boundary_FileName(string plateLabel, int stg_young, int stg_old)
        {
            return $"BDR_{plateLabel}_{stg_young}_{stg_old}.txt";
        }


        public static string Set_BoundaryIn_FileName(string plateLabel, int stg_young, int stg_old)
        {
            return $"BDRin_{plateLabel}_{stg_young}_{stg_old}.txt";
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

        public static string Set_ContourCoordinates_FileName(InputParameters inputParams, double percentageLevel)
        {
            string CNTR_PDD_LBL = Set_ContourCoordinates_Label(inputParams);

            if (percentageLevel.ToString().Contains(','))
                CNTR_PDD_LBL = $"CNTR" + string.Format(percentageLevel.ToString()).Replace(",", "p") + CNTR_PDD_LBL + ".txt";
            else
                CNTR_PDD_LBL = $"CNTR" + string.Format(percentageLevel.ToString()).Replace(".", "p") + CNTR_PDD_LBL + ".txt";

            return CNTR_PDD_LBL;
        }


        public static string Set_ContourCoordinates_Label(InputParameters inputParams)
        {
            double gStep = inputParams.DM_CNTR_BINS[0];
            string CNTR_PDD_LBL = $"_{inputParams.RUN_LABEL}_r";

            if (gStep.ToString().Contains(','))
                CNTR_PDD_LBL += string.Format(gStep.ToString()).Replace(",", "p");
            else
                CNTR_PDD_LBL += string.Format(gStep.ToString()).Replace(".", "p");

            return CNTR_PDD_LBL;
        }

        public static string Set_MagnitudeHistogram_Label(InputParameters inputParams)
        {
            return $"{inputParams.RUN_LABEL}_{inputParams.DM_MAGHIST_BINS}";
        }

        public static string Set_MagnitudeHistogram_FileName(InputParameters inputParams)
        {
            string label = Set_MagnitudeHistogram_Label(inputParams);
            return $"MAGHIST_{label}.txt";
        }
    }
}
