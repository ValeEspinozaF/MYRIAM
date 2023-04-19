using StructOperations;
using DataStructures;
using static MYRIAM.FileReader;


namespace MYRIAM
{
    class Manage_InputParams
    {
        public static Dictionary<string, object> Set_InputParams(string inputFilePath)
        {
            // Upload input file
            var givenStrParams = File_toInputStrings(inputFilePath);


            // Fill missing parameters with default ones
            var inputStrParams = UpdateParams(givenStrParams);


            // Process parameters to types
            var inputParams = InputStrings_toParams(inputStrParams);


            // If EVo_PATH is not given (e.g. for absolute plate motions), set empty string instead
            if (!inputParams.ContainsKey("EVo_PATH"))
                inputParams.Add("EVo_PATH", null);


            // If contour path (CTR_PATH) is not given, use Matthews et al. 2016 Present-day contour
            if (!inputParams.ContainsKey("CTR_PATH"))
                inputParams.Add("CTR_PATH", GetContourPath((string) inputParams["PLT_LABEL"]));


            // If python path (PYTHON_PATH) is not given, set empty string instead
            if (!inputParams.ContainsKey("PYTHON_PATH"))
                inputParams.Add("PYTHON_PATH", null);


            return inputParams;
        }


        public static Dictionary<string, string> UpdateParams(Dictionary<string, string> givenParams)
        {
            // Default parameters
            var defaultParams = Load_defaultParams();

            // Merge given and default parameters
            var mergedParams = givenParams.Concat(defaultParams.
                                                  Where(kvp => !givenParams.ContainsKey(kvp.Key))).
                                                  ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return mergedParams;
        }


        public static Dictionary<string, string> Load_defaultParams()
        {
            string pathDefault = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\assets\TextFiles\DefaultParameters.txt");

            return File_toInputStrings(pathDefault);
        }


        public static Dictionary<string, object> InputStrings_toParams(Dictionary<string, string> inputStrings)
        {
            // Set output dictionary
            var inputVars = new Dictionary<string, object>();


            // Iterate over dictionary
            foreach (KeyValuePair<string, string> entry in inputStrings)
            {
                string[] values = entry.Value
                    .Split(new char[] { '[', '(', '{', ',', ' ', '\t', ';', '}', ')', ']' }, StringSplitOptions.RemoveEmptyEntries);

                switch (entry.Key)
                {
                    case "SAVE_ENS":
                        {
                            try
                            {
                                inputVars.Add(entry.Key, bool.Parse(entry.Value));
                            }
                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be set to either true or false."
                                    );
                            }
                            break;
                        }

                    case "OUTPUTS_DIR":
                        {
                            if (entry.Value == "")
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Directory path is empty. " +
                                    $"Fix this by supplying a directory path or omitting the parameter."
                                    );

                            if (!Directory.Exists(entry.Value))
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Path to directory does not exist."
                                    );

                            else
                                inputVars.Add(entry.Key, entry.Value);

                            break;
                        }

                    case "EVy_PATH":
                    case "EVo_PATH":
                    case "CTR_PATH":
                    case "PYTHON_PATH":
                        {
                            if (entry.Value == "")
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"File path is empty. " +
                                    $"Fix this by supplying a file path or omitting the parameter."
                                    );

                            else if (!File.Exists(entry.Value))
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Path to file does not exist."
                                    );

                            inputVars.Add(entry.Key, entry.Value);

                            break;
                        }

                    case "PLT_LABEL":
                        {
                            if (entry.Value == "")
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Label entry is empty. " +
                                    $"Fix this by supplying a label."
                                    );

                            inputVars.Add(entry.Key, entry.Value);

                            break;
                        }

                    case "OUTPUT_LABEL":
                        {
                            string stringValue = string.Join("", entry.Value.Split('\\', '/', ':'));
                            bool overwrite_output = false;

                            if (entry.Value != "" && entry.Value.Last() == '!')
                            {
                                stringValue = stringValue.Substring(0, stringValue.Length - 1);
                                overwrite_output = true;
                            }

                            inputVars.Add(entry.Key, stringValue);
                            inputVars.Add("OVERWRT_OUTPUT", overwrite_output);

                            break;
                        }

                    case "INTERP_MTD":
                        {
                            if (entry.Value == "")
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Interpolation method entry is empty. " +
                                    $"Fix this by supplying a string ('linear' or 'nearest') or omitting the parameter."
                                    );

                            inputVars.Add(entry.Key, entry.Value);

                            break;
                        }

                    case "FRACTION_HA":
                        {
                            try
                            {
                                double doubleValue = GetValue(entry.Value);

                                if (doubleValue <= 0 || doubleValue > 1)
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Value must be between zero and one (excluding zero)."
                                        );

                                inputVars.Add(entry.Key, doubleValue);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be a single number. " +
                                    $"Fix this by supplying a value or omitting the parameter."
                                    );
                            }
                            break;
                        }

                    case "HL":
                    case "muM":
                    case "muA":
                        {
                            try
                            {
                                double doubleValue = GetValue(entry.Value);

                                if (doubleValue < 0.0)
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Value cannot be a negative number."
                                        );

                                else if (doubleValue == 0.0)
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Value cannot be set to zero."
                                        );

                                inputVars.Add(entry.Key, doubleValue);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be a single number. " +
                                    $"Fix this by supplying a value or omitting the parameter."
                                    );
                            }
                            break;
                        }                

                    case "GRID_RES":
                        {
                            try
                            {
                                double doubleValue = GetValue(entry.Value);

                                if (doubleValue <= 0.0)
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Value must higher than zero."
                                        );

                                inputVars.Add(entry.Key, doubleValue);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be a single number. " +
                                    $"Fix this by supplying a value or omitting the parameter."
                                    );
                            }
                            break;
                        }

                    case "DEF_DISTANCE":
                        {
                            try
                            {
                                double doubleValue = GetValue(entry.Value);

                                if (doubleValue < 0.0)
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Value cannot be a negative number."
                                        );

                                inputVars.Add(entry.Key, doubleValue);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be a single number. " +
                                    $"Fix this by supplying a value or omitting the parameter."
                                    );
                            }
                            break;
                        }

                    case "REGION_muA_LV":
                        {
                            try
                            {
                                double[] double1DArray = (from val in values select GetValue(val)).ToArray();

                                if (double1DArray.Length == 0)
                                    double1DArray = ArrayManagement.Zeros1D(4, 0.0);

                                else if (double1DArray.Length == 4)
                                {
                                    if (double1DArray[0] > double1DArray[1] || double1DArray[2] > double1DArray[3])
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Coordinates out of order [lonMin, lonMax, latMin, latMax]."
                                            );

                                    else if (double1DArray[0] == double1DArray[1] || double1DArray[2] == double1DArray[3])
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Coordinates [lonMin, lonMax, latMin, latMax] yield no area."
                                            );

                                    else if (double1DArray[0] < -180 || double1DArray[1] > 180 || double1DArray[2] < -90 || double1DArray[3] > 90)
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Coordinates out of global bounds [-180, 180,-90, 90]."
                                            );
                                }

                                else 
                                {
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Unsupported amount of values, must supply either 4 or none."
                                        );
                                }

                                inputVars.Add(entry.Key, new CoordsLimits
                                {
                                    LonMin = double1DArray[0],
                                    LonMax = double1DArray[1],
                                    LatMin = double1DArray[2],
                                    LatMax = double1DArray[3]
                                });
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be 4 numbers.");
                            }

                            break;
                        }

                    case "ANG_ROT":
                        {
                            try
                            {
                                double[] double1DArray = (from val in values select GetValue(val)).ToArray();


                                if (double1DArray.Length == 3)
                                {
                                    if (double1DArray[0] < -180 || double1DArray[0] > 180)
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Rotation angle for Z-axis is out of longitude bounds [-180, 180]."
                                            );

                                    if (double1DArray[1] < -90 || double1DArray[1] > 90)
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Rotation angle for Y-axis is out of latitude bounds [-90, 90]."
                                            );

                                    inputVars.Add(entry.Key, double1DArray);
                                }

                                else
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Unsupported amount of values, must supply either 3 or none."
                                        );
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be 3 numbers.");
                            }

                            break;
                        }

                    case "DM_MAGHIST_BINS":
                        {
                            int intValue;

                            try
                            {
                                if (values.Length == 0)
                                    intValue = 50;

                                else if (values.Length == 1)
                                {
                                    intValue = (int)GetValue(values[0]);

                                    if (intValue <= 0)
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Value must higher than zero."
                                            );
                                }

                                else
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Unsupported amount of values, must supply either 1 or none."
                                        );

                                inputVars.Add(entry.Key, intValue);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Entry must be a single number. " +
                                    $"Fix this by supplying a value or omitting the parameter."
                                    );
                            }
                            break;
                        }

                    case "DM_CNTR_BINS":
                        {
                            try
                            {
                                double[] double1DArray = (from val in values select GetValue(val)).ToArray();


                                if (double1DArray.Length == 0)
                                {
                                    double1DArray = ArrayManagement.Reshape1D(double1DArray, double.NaN, 5);
                                }

                                else if (double1DArray.Length == 1)
                                {
                                    double1DArray = ArrayManagement.Reshape1D(double1DArray, double.NaN, 5);

                                    if (double1DArray[0] <= 0)
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Grid resolution must be bigger than zero."
                                            );
                                }

                                else if (double1DArray.Length == 5)
                                {
                                    if (double1DArray[1] > double1DArray[2] || double1DArray[3] > double1DArray[4])
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Coordinates out of order [lonMin, lonMax, latMin, latMax]."
                                            );

                                    if (double1DArray[1] < -180 || double1DArray[2] > 180 || double1DArray[3] < -90 || double1DArray[4] > 90)
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Coordinates out of global bounds [-180, 180,-90, 90]."
                                            );

                                    if (double1DArray[1] == double1DArray[2] || double1DArray[3] == double1DArray[4])
                                        throw new InputErrorException(
                                            $"Error in {entry.Key} syntax. " +
                                            $"Coordinates [lonMin, lonMax, latMin, latMax] yield no area."
                                            );
                                }

                                else
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Unsupported amount of values, must supply either 5, 1 or none."
                                        );

                                inputVars.Add(entry.Key, double1DArray);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Values must be numbers.");
                            }

                            break;
                        }

                    case "DM_CNTR_PERCENT":
                        {
                            if (entry.Value == "")
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Parameter is empty. " +
                                    $"Fix this by supplying one or more percentage values or omitting the parameter."
                                    );

                            try
                            {
                                double[] double1DArray = (from val in values select GetValue(val)).ToArray();

                                inputVars.Add(entry.Key, double1DArray);
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Values must be numbers.");
                            }

                            break;
                        }

                    case "STG_IDXs":
                        {
                            try
                            {
                                double[] double1DArray = (from val in values select GetValue(val)).ToArray();

                                if (values.Length != 2)
                                {
                                    throw new InputErrorException(
                                        $"Error in {entry.Key} syntax. " +
                                        $"Must provide 2 integers separated by comma."
                                        );
                                }

                                else
                                {
                                    try
                                    {
                                        int stg1 = int.Parse(values[0]);
                                        int stg2 = int.Parse(values[1]);
                                        inputVars.Add(entry.Key, new int[2] { stg1, stg2 });
                                    }
                                    catch (FormatException)
                                    {
                                        int stg1 = int.Parse(values[0].Split(new char[] { '.' })[0]);
                                        int stg2 = int.Parse(values[1].Split(new char[] { '.' })[0]);
                                        inputVars.Add(entry.Key, new int[2] { stg1, stg2 });
                                    }
                                }
                            }

                            catch (FormatException)
                            {
                                throw new InputErrorException(
                                    $"Error in {entry.Key} syntax. " +
                                    $"Values must be numbers.");
                            }

                            break;
                        }
                }
            }

            // Additional actions
            if (inputVars.ContainsKey("PYTHON_PATH"))
            {
                double[] percentArray = (double[])inputVars["DM_CNTR_PERCENT"];

                if (!percentArray.Contains(68))
                {
                    inputVars["DM_CNTR_PERCENT"] = percentArray.Concat(new double[] { 68 }).ToArray();
                    
                    Console.WriteLine(
                        "Warning! " +
                        "Adding 68% confidence percentage (for diagnostics figures)");
                }

                if (!percentArray.Contains(20))
                {
                    inputVars["DM_CNTR_PERCENT"] = percentArray.Concat(new double[] { 20 }).ToArray();

                    Console.WriteLine(
                        "Warning! " +
                        "Adding 20% confidence percentage (for diagnostics figures)");
                }
            }

            return inputVars;
        }


        public static void Load_EV_Ensembles(string EVo_Path, string EVy_Path,
                                             out vectorCart[] EVo, out vectorCart[] EVy)
        {
            EVo = new vectorCart[0];
            EVy = new vectorCart[0];

            // ========== Young Euler vectors ===================

            // Load file
            double[,] EVy_array = File_toArray(EVy_Path);


            // If data is an ensemble, store it
            if (EVy_array.GetLength(0) != 1)
            {
                EVy = VectorOperations.ArrayToCartesianVector(EVy_array);
            }


            // If data is an array, build ensemble
            else if (EVy_array.GetLength(0) == 1)
            {
                double[] evArray = ArrayManagement.ArrayFlatten(EVy_array, 0);
                EVy = Generate_EV_Ensemble.Generate_EuVectorEnsemble(evArray);
            }

            int length = EVy.GetLength(0);
            EVy_array = null;


            // ========== Old Euler vectors ===================

            // If file path is empty, build an ensemble of zeros
            if (string.IsNullOrEmpty(EVo_Path))
            {
                double[,] EVo_array = ArrayManagement.Zeros2D(length, 3, (double)0);
                EVo = VectorOperations.ArrayToCartesianVector(EVo_array);
            }

            // If file path is not empty, load data
            else
            {
                double[,] EVo_array = File_toArray(EVo_Path);


                // If data is an ensemble, store it
                if (EVo_array.GetLength(0) != 1)
                {
                    EVo = VectorOperations.ArrayToCartesianVector(EVo_array);
                }


                // If data is an array, build ensemble
                else if (EVo_array.GetLength(0) == 1)
                {
                    double[] evArray = ArrayManagement.ArrayFlatten(EVo_array, 0);
                    EVo = Generate_EV_Ensemble.Generate_EuVectorEnsemble(evArray);
                }
            }
        }
    }
}
