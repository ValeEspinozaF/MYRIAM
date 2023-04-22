using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EulerVector
{
    public class VectorSph
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Magnitude { get; set; }
        public Covariance Covariance { get; set; }

        public VectorSph()
        {
        }

        public VectorSph(double lon, double lat, double ang)
        {
            Longitude = lon;
            Latitude = lat;
            Magnitude = ang;
        }

        public double[] Values()
        {
            return new double[] { Longitude, Latitude, Magnitude };
        }

        public static void GetVectorColumns(VectorSph[] vectorArray, out double[] lon, out double[] lat, out double[] mag)
        {
            lon = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Longitude).ToArray();
            lat = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Latitude).ToArray();
            mag = Enumerable.Range(0, vectorArray.Length).Select(x => vectorArray[x].Magnitude).ToArray();
        }
    }
}
