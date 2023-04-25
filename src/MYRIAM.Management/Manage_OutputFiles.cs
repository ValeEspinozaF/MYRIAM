using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructOperations;
using DataStructures;
using static StructOperations.ArrayManagement;
using Torque;
using Cartography;
using static MYRIAM.FileReader;


namespace MYRIAM
{
    partial class ManageOutputs
    {
        public static void Create_OutputDirs(
            InputParameters inputParams,
            out string dir_MTXwM, out string dir_dM_PDD, out string dir_TMP)
        {
            string outputDir = inputParams.OUTPUTS_DIR;
            string pltLabel = inputParams.PLT_LABEL;
            string outputLabel = inputParams.OUTPUT_LABEL;
            bool overwriteOutput = inputParams.OVERWRT_OUTPUT;


            dir_MTXwM = Path.Combine(outputDir, $"REPOSITORY_{pltLabel}_MTX_w2M");
            dir_dM_PDD = Path.Combine(outputDir, $"REPOSITORY_{pltLabel}_dM_PDD");
            dir_TMP = Path.Combine(outputDir, $"REPOSITORY_{pltLabel}_TMP");

            if (!string.IsNullOrEmpty(outputLabel))
            {                
                dir_MTXwM = dir_MTXwM + "_" + outputLabel;
                dir_dM_PDD = dir_dM_PDD + "_" + outputLabel;
                dir_TMP = dir_TMP + "_" + outputLabel;
            }

            foreach (var dir in new List<string> { dir_MTXwM, dir_dM_PDD, dir_TMP })
            {
                try
                {
                    // Check if directory already exists
                    if (Directory.Exists(dir))
                    {
                        if (!overwriteOutput)
                        {
                            Console.Write("\nRepository folder already exists, files may be overwriten. Do you want to continue [Lat/n]? ");

                            char answer;
                            char[] yes = { 'Y', 'y' };
                            char[] no = { 'N', 'n' };

                            if (char.TryParse(Console.ReadLine(), out answer))
                            {
                                if (no.Contains(answer))
                                    Environment.Exit(0);
                                else if (yes.Contains(answer))
                                    overwriteOutput = true;
                                else
                                    throw new Exception("Invalid input character, please type Lat for yes, n for no.");
                            }
                            else
                            {
                                throw new Exception("Invalid input character, please type Lat for yes, n for no.");
                            }

                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.Write(new string(' ', Console.WindowWidth));
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            //Console.Write(new string(' ', Console.WindowWidth));
                        }
                    }
                    else
                    {
                        // Create the directory
                        Directory.CreateDirectory(dir);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Directory creation failed: {0}", e.ToString());
                }
            }
        }


        public static void Delete_TMPs(string dir_TMP)
        {
            var dir = new DirectoryInfo(dir_TMP);
            dir.Delete(true);
        }

        public static void Save_toTXT(double value, string Dir, string FileName, string format = "F1")
        {
            List<string> linesList = new()
            {
                SetString(value, format)
            };

            string filePath = Path.Combine(Dir, FileName);
            File.WriteAllLines(filePath, linesList);
        }


        public static void Save_toTXT(double[] array, string dir, string fileName, string format = "#.####E+0")
        {
            List<string> linesList = new()
            {
                SetString(array, format)
            };

            string filePath = Path.Combine(dir, fileName);
            File.WriteAllLines(filePath, linesList);
        }


        public static void Save_toTXT(double[,] array, string fileDir, string fileName, string format = "#.####E+0")
        {
            List<string> linesList = new();

            for (int i = 0; i < array.GetLength(0); i++)
            {
                var row = GetRow(array, i);
                linesList.Add(SetString(row, format));
            }

            SaveLines_asTXT(linesList, fileDir, fileName);
        }


        public static void Save_toTXT(double[,] array, string fileDir, string fileName, string[] format)
        {
            if (array.GetLength(1) != format.Length)
                throw new ArgumentException("Length of format must be same as number of columns in array.");
            List<string> linesList = new();

            for (int i = 0; i < array.GetLength(0); i++)
            {
                var row = GetRow(array, i);

                string line = "";
                for (int k = 0; k < row.Length; k++)
                {
                    line += " " + SetString(row[k], format[k]);
                    line = line.Trim();
                }

                linesList.Add(line);
            }

            SaveLines_asTXT(linesList, fileDir, fileName);
        }


        public static void Save_toTXT(Coordinate[] Array, string fileDir, string fileName, string format = "0.000")
        {
            List<string> linesList = new();

            for (int i = 0; i < Array.Length; i++)
            {
                linesList.Add(string.Join(" ", new string[] {
                    SetString(Array[i].Lon, format),
                    SetString(Array[i].Lat, format)
                }));
            }

            SaveLines_asTXT(linesList, fileDir, fileName);
        }


        public static void Save_toTXT(TorqueVector[] array, string fileDir, string fileName, string format = "0.000")
        {
            string filePath = Path.Combine(fileDir, fileName);

            StringBuilder stringbuilder = new ();
            for (int i = 0; i < array.Length; i++)
            {
                double[] values = new double[] { array[i].X, array[i].Y, array[i].Z };
                stringbuilder.Append(SetString(values, format) + "\n");
            }

            File.WriteAllText(filePath, stringbuilder.ToString());
        }


        public static string SetString(int value, string format = "0.000")
        {
            return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }


        public static string SetString(double value, string format = "0.000")
        {
            return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }


        public static string SetString(int[] values, string format = "0.000", string delimiter = " ")
        {
            return string.Join(delimiter, from value in values select SetString(value, format));
        }


        public static string SetString(double[] values, string format = "0.000", string delimiter = " ")
        {
            return string.Join(delimiter, from value in values select SetString(value, format));
        }


        public static void SaveLines_asTXT(List<string> linesList, string fileDir, string fileName)
        {
            string filePath = Path.Combine(fileDir, fileName);
            File.WriteAllLines(filePath, linesList);
        }

        public static string Get_pyScriptPath(string pyScriptName)
        {
            string dirDefault = Get_ItemPath("assets");
            string pyScriptsDir = Path.Combine(dirDefault, "PythonFunctions");
            return Path.Combine(pyScriptsDir, pyScriptName);
        }


        public static void Generate_gridFigures(string pythonPath, string dir_MTXwM, string modelLabel, string boundaryLabel)
        {
            string args = string.Format("{0} {1} {2}", modelLabel, boundaryLabel, dir_MTXwM);
            string pyScriptPath = Get_pyScriptPath("GridMaps.py");

            PythonCaller.Run_pythonFile(pythonPath, pyScriptPath, args);
        }


        public static void Generate_histogramFigure(string pythonPath, string dir_dM_PDD, string histLabel)
        {
            string args = string.Format("{0} {1}", histLabel, dir_dM_PDD);
            string pyScriptPath = Get_pyScriptPath("MagnitudeHistogram.py");

            PythonCaller.Run_pythonFile(pythonPath, pyScriptPath, args);
        }


        public static void Generate_contourFigure(string pythonPath, string dir_MTXwM, string dir_dM_PDD, string contourLabel, string plateLabel)
        {
            string args = string.Format("{0} {1} {2} {3}", contourLabel, plateLabel, dir_MTXwM, dir_dM_PDD);
            string pyScriptPath = Get_pyScriptPath("ContourMap.py");

            PythonCaller.Run_pythonFile(pythonPath, pyScriptPath, args);
        }


        public static void Generate_rotatedCntr_Figure(string pythonPath, string plateLabel, string dir_TMP, string dir_MTXwM, string dir_dM_PDD)
        {
            string args = string.Format("{0} {1} {2} {3}", plateLabel, dir_TMP , dir_MTXwM, dir_dM_PDD);
            string pyScriptPath = Get_pyScriptPath("RotatedEnsembleMap.py");

            PythonCaller.Run_pythonFile(pythonPath, pyScriptPath, args);
        }


        public static void Save_InputParamsReport(InputParameters inputParams)
        {
            // Upload Template file
            string dirDefault = Get_ItemPath("assets");
            string pathReport = Path.Combine(dirDefault, "TextFiles/InputFileReport_Template.txt");
            string[] lines = File.ReadAllLines(pathReport);
            

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "" && lines[i][0].ToString() != "!" && lines[i][0].ToString() != @"\")
                {
                    string key = lines[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();


                    // Deal with empty entries (null)
                    if (!inputParams.ContainsKey(key))
                        lines[i] = "!" + lines[i];

                    else
                    {
                        switch (key)
                        {
                            case "SAVE_ENS":
                                {
                                    bool value = (bool)inputParams.GetValue(key);
                                    lines[i] = lines[i] + $" {value}";

                                    break;
                                }

                            case "PLT_LABEL":
                            case "EVo_PATH":
                            case "EVy_PATH":
                            case "OUTPUTS_DIR":
                            case "INTERP_MTD":
                            case "PYTHON_PATH":
                            case "CTR_PATH":
                            case "OUTPUT_LABEL":
                                {
                                    string value = (string)inputParams.GetValue(key);

                                    if (value != null)
                                        lines[i] = lines[i] + $" {value}";

                                    break;
                                }

                            case "GRID_RES":
                            case "HL":
                            case "FRACTION_HA":
                            case "DEF_DISTANCE":
                                {
                                    double value = (double)inputParams.GetValue(key);
                                    lines[i] = lines[i] + $" {SetString(value, "F1")}";

                                    break;
                                }

                            case "muM":
                            case "muA":
                                {
                                    double value = (double)inputParams.GetValue(key);
                                    lines[i] = lines[i] + $" {SetString(value, "#.##E+0")}";

                                    break;
                                }

                            case "DM_MAGHIST_BINS":
                                {
                                    int value = (int)inputParams.GetValue(key);
                                    lines[i] = lines[i] + $" {SetString(value, "F1")}";

                                    break;
                                }

                            case "DM_CNTR_BINS":
                            case "DM_CNTR_PERCENT":
                            case "ANG_ROT":
                                {
                                    var values = (double[])inputParams.GetValue(key);

                                    if (values != null)
                                    {
                                        string valueString = SetString(values, format: "F1", delimiter: ", ");
                                        lines[i] = lines[i] + " [" + valueString + "]";
                                    }

                                    break;
                                }

                            case "STG_IDXs":
                                {
                                    int[] values = (int[])inputParams.GetValue(key);
                                    lines[i] = lines[i] + $" {SetString(values, format: "F0", delimiter: ", ")}";

                                    break;
                                }

                            case "REGION_muA_LV":
                                {
                                    CartoLimits value = (CartoLimits)inputParams.GetValue(key);
                                    lines[i] = lines[i] + $" [" +
                                        $"{SetString(value.LonMin, "F1")}, " +
                                        $"{SetString(value.LonMax, "F1")}, " +
                                        $"{SetString(value.LatMin, "F1")}, " +
                                        $"{SetString(value.LatMax, "F1")}]";

                                    break;
                                }
                        }
                    }
                }
            }
            string fileName = 
                $"INPUT_FILE_REPORT_{inputParams.PLT_LABEL}" +
                $"_{inputParams.STG_IDX_1}" +
                $"_{inputParams.STG_IDX_2}" +
                $"_{inputParams.OUTPUT_LABEL}" +
                $".txt";
            
            SaveLines_asTXT(lines.ToList(), inputParams.OUTPUTS_DIR, fileName);
        }
    }
}
