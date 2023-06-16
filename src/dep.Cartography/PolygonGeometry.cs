using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;


namespace Cartography
{
    public partial class PolygonGeometry
    {
        public static Coordinate[] Clean_coordDuplicates(Coordinate[] cntrArray)
        {
            // Get differential function from coordinates
            var diffArray = from i in Enumerable.Range(0, cntrArray.Length - 1)
                            select Math.Abs(
                                cntrArray[i + 1].Lon - cntrArray[i].Lon) + 
                                Math.Abs(cntrArray[i + 1].Lat - cntrArray[i].Lat
                                );


            // Retrieve index of items where difference is zero
            var zeroPair = diffArray
                .Select((element, index) => new KeyValuePair<double, double>(index, element))
                .Where(x => x.Value.Equals(0))
                .ToArray();

            var zeroArray = (from value in zeroPair select value.Key).ToArray();


            // Filter duplicate longitude coordinates 
            var coordsPair = cntrArray
                .Select((element, index) => new KeyValuePair<double, Coordinate>(index, element))
                .Where(p => zeroArray.All(p2 => p2 != p.Key - 1))
                .ToArray();

            var coordsList = (from value in coordsPair select value.Value).ToList();


            // Close boundary by duplicating first value at end of array
            if (coordsList[0].Lon != coordsList.Last().Lon || coordsList[0].Lat != coordsList.Last().Lat)
                coordsList.Add(coordsList[0]);


            // Turn list to array
            return coordsList.ToArray();
        }


        public static Coordinate[] Points_InPolygon(Coordinate[] polygonPoints, Coordinate[] testPoints)
        {
            return testPoints.Where(x => Point_InPolygon(polygonPoints, x)).ToArray();
        }


        public static bool Point_InPolygon(Coordinate[] polygon, Coordinate point)
        {
            int n = polygon.Length;

            // Create a point for line segment from p to infinite (a big value)
            Coordinate point2 = Coordinate.FromUnit(10000, point.Lat + 0.00000001, (CoordinateUnit)polygon[0]._unit);


            // Count intersections of the above line with sides of polygon
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % n;

                // Check if the line 'point-point2' intersects the segment 'polygon[i]-polygon[next]'
                if (doIntersect(polygon[i], polygon[next], 
                                point, point2))
                {
                    // If the point is collinear with the segment 'i-next',
                    if (orientation(polygon[i], point, polygon[next]) == 0)
                    {
                        // then check and return true if it lies on the segment.
                        return onSegment(polygon[i], point, polygon[next]);
                    }
                    count++;
                }
                i = next;
            } while (i != 0);

            // Return true if count is odd, false otherwise
            return (count % 2 == 1);
        }

        private static bool doIntersect(Coordinate p1, Coordinate q1, Coordinate p2, Coordinate q2)
        {
            // Return true if line segment 'p1q1' and 'p2q2' intersect.


            // Find the four orientations needed for general and special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases
            // p1, q1 and p2 are collinear and
            // p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are collinear and
            // q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are collinear and
            // p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are collinear and
            // q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2))
            {
                return true;
            }

            // Doesn't fall in any of the above cases
            return false;
        }

        private static bool onSegment(Coordinate p, Coordinate q, Coordinate r)
        {
            // Checks if point 'q' lies on line segment 'pr'
            if (q.Lon <= Math.Max(p.Lon, r.Lon) &&
                q.Lon >= Math.Min(p.Lon, r.Lon) &&
                q.Lat <= Math.Max(p.Lat, r.Lat) &&
                q.Lat >= Math.Min(p.Lat, r.Lat))
            {
                return true;
            }
            return false;
        }

        
        private static int orientation(Coordinate p, Coordinate q, Coordinate r)
        {
            // To find orientation of ordered triplet (p, q, r).
            // The function returns following values
            // 0 --> p, q and r are collinear
            // 1 --> Clockwise
            // 2 --> Counterclockwise

            double val = (q.Lat - p.Lat) * (r.Lon - q.Lon) - (q.Lon - p.Lon) * (r.Lat - q.Lat);

            if (val == 0)
            {
                return 0; //  is collinear
            }
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }
    }
}
