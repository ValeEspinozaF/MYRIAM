using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class Covariance
    {
        public double C11;
        public double C12;
        public double C13;
        public double C22;
        public double C23;
        public double C33;

        public Covariance()
        {
        }

        public Covariance(double a, double b, double c, double d, double e, double f)
        {
            C11 = a;
            C12 = b;
            C13 = c;
            C22 = d;
            C23 = e;
            C33 = f;
        }

        public Covariance(double[] a_to_f)
        {
            C11 = a_to_f[0];
            C12 = a_to_f[1];
            C13 = a_to_f[2];
            C22 = a_to_f[3];
            C23 = a_to_f[4];
            C33 = a_to_f[5];
        }

        public double[] Values()
        {
            return new double[] { C11, C12, C13, C22, C23, C33 };
        }

        public double[,] ToMatrix()
        {
            return new double[3, 3]
            {
                { C11, C12, C13 },
                { C12, C22, C23 },
                { C13, C23, C33 }
            };
        }

        public static void GetCovarianceColumns(Covariance[] cov,
                                                out double[] C11, out double[] C12, out double[] C13,
                                                out double[] C22, out double[] C23, out double[] C33)
        {
            C11 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C11).ToArray();
            C12 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C12).ToArray();
            C13 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C13).ToArray();
            C22 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C22).ToArray();
            C23 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C23).ToArray();
            C33 = Enumerable.Range(0, cov.Length).Select(x => cov[x].C33).ToArray();
        }
    }
}
