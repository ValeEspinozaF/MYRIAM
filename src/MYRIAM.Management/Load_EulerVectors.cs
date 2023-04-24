using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructOperations;
using DataStructures;


namespace MYRIAM
{
    internal class Load_EulerVector
    {
        public static void Load_EulerVectors(string EVo_Path, string EVy_Path,
                                             out EulerVector[] EVo, out EulerVector[] EVy)
        {
            EVo = new EulerVector[0];
            EVy = new EulerVector[0];

            // ========== Young Euler vectors ===================

            // Load file
            double[,] EVy_array = FileReader.File_toArray(EVy_Path);


            // If data is an ensemble, store it
            if (EVy_array.GetLength(0) != 1)
            {
                EVy = EulerVector.ArrayToCartesianVector(EVy_array);
            }


            // If data is an array, build ensemble
            else if (EVy_array.GetLength(0) == 1)
            {
                double[] evArray = ArrayManagement.ArrayFlatten(EVy_array, 0);
                EVy = EulerVector.Generate_Ensemble(evArray);
            }

            int length = EVy.GetLength(0);
            EVy_array = null;


            // ========== Old Euler vectors ===================

            // If file path is empty, build an ensemble of zeros
            if (string.IsNullOrEmpty(EVo_Path))
            {
                double[,] EVo_array = ArrayManagement.Zeros2D(length, 3, (double)0);
                EVo = EulerVector.ArrayToCartesianVector(EVo_array);
            }

            // If file path is not empty, load data
            else
            {
                double[,] EVo_array = FileReader.File_toArray(EVo_Path);


                // If data is an ensemble, store it
                if (EVo_array.GetLength(0) != 1)
                {
                    EVo = EulerVector.ArrayToCartesianVector(EVo_array);
                }


                // If data is an array, build ensemble
                else if (EVo_array.GetLength(0) == 1)
                {
                    double[] evArray = ArrayManagement.ArrayFlatten(EVo_array, 0);
                    EVo = EulerVector.Generate_Ensemble(evArray);
                }
            }
        }
    }
}
