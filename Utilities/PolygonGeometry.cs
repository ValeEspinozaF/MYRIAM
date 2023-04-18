using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;


namespace ComputationalGeometry
{
    class PolygonGeometry
    {
        public static Coord[] Points_InPolygon(Coord[] polygonPoints, Coord[] testPoints)
        {
            return testPoints.Where(x => Point_InPolygon(polygonPoints, x)).ToArray();
        }


        public static bool Point_InPolygon(Coord[] polygon, Coord point)
        {
            int n = polygon.Length;

            // Create a point for line segment from p to infinite (a big value)
            Coord point2 = new(10000, point.Y + 0.00000001);


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

        private static bool doIntersect(Coord p1, Coord q1, Coord p2, Coord q2)
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

        private static bool onSegment(Coord p, Coord q, Coord r)
        {
            // Checks if point 'q' lies on line segment 'pr'
            if (q.X <= Math.Max(p.X, r.X) &&
                q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) &&
                q.Y >= Math.Min(p.Y, r.Y))
            {
                return true;
            }
            return false;
        }

        
        private static int orientation(Coord p, Coord q, Coord r)
        {
            // To find orientation of ordered triplet (p, q, r).
            // The function returns following values
            // 0 --> p, q and r are collinear
            // 1 --> Clockwise
            // 2 --> Counterclockwise

            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0)
            {
                return 0; //  is collinear
            }
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }
    }
}
