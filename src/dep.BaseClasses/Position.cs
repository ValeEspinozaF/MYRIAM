using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class GridXY
    {
        public GridXY(int x, int y)
        {
            Y = y;
            X = x;
        }
        public int Y { get; set; }
        public int X { get; set; }
    }

    public class PositionXY
    {
        public PositionXY(double x, double y)
        {
            Y = y;
            X = x;
        }
        public double Y { get; set; }
        public double X { get; set; }
    }
}
