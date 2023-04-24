using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;

namespace Torque
{
    public class TorqueVector : Vector
    {
        public TorqueVector()
        {
        }

        public static TorqueVector[] ToCartesian(TorqueVector[] vectorArray)
        {
            for (int i = 0; i < vectorArray.Length; i++)
                vectorArray[i].ToCartesian();

            return vectorArray;
        }


        public static TorqueVector[] ToSpherical(TorqueVector[] vectorArray)
        {
            for (int i = 0; i < vectorArray.Length; i++)
                vectorArray[i].ToSpherical();

            return vectorArray;
        }

        public static void GetCartesianColumns(TorqueVector[] vectorArray, out double[] X, out double[] Y, out double[] Z)
        {
            X = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].X).ToArray();
            Y = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Y).ToArray();
            Z = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Z).ToArray();
        }
    }
}
