using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using static StructOperations.ArrayOperations;

namespace MYRIAM.Torque
{
    internal class TorqueStatistics
    {
        public static TorqueVector EnsembleMean(TorqueVector[] ens, bool cov = false)
        {
            TorqueVector.GetCartesianColumns(ens, out double[] colX, out double[] colY, out double[] colZ);

            if (cov == true)
            {
                int N = ens.Length;

                // Set covariance values [units^2]
                Covariance covArray = new()
                {
                    C11 = Sum(ArrayProduct(colX, colX)) / N - Sum(colX) / N * Sum(colX) / N,
                    C12 = Sum(ArrayProduct(colX, colY)) / N - Sum(colX) / N * Sum(colY) / N,
                    C13 = Sum(ArrayProduct(colX, colZ)) / N - Sum(colX) / N * Sum(colZ) / N,
                    C22 = Sum(ArrayProduct(colY, colY)) / N - Sum(colY) / N * Sum(colY) / N,
                    C23 = Sum(ArrayProduct(colY, colZ)) / N - Sum(colY) / N * Sum(colZ) / N,
                    C33 = Sum(ArrayProduct(colZ, colZ)) / N - Sum(colZ) / N * Sum(colZ) / N,
                };

                return new TorqueVector
                {
                    X = colX.Average(),
                    Y = colY.Average(),
                    Z = colZ.Average(),
                    Covariance = covArray
                };
            }
            else
            {
                return new TorqueVector
                {
                    X = colX.Average(),
                    Y = colY.Average(),
                    Z = colZ.Average(),
                };
            }
        }
    }
}
