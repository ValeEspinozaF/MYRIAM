using static CartographicCoordinates.TransformSystem;
using ComputationalGeometry;
using DataStructures;
using Utilities;



namespace CartographicCoordinates
{
    class ManageCoordinates
    {
        public static CoordsLimits Extract_CoordsLimits(Coord[] coordsArray)
        {
            // Clone coordinates list
            var sortedList = new List<Coord>(coordsArray.ToList());

            // Sort longitudes
            sortedList.Sort((x, y) => x.X.CompareTo(y.X));
            double lonMin = sortedList[0].X;
            double lonMax = sortedList[^1].X;

            sortedList.Sort((x, y) => x.Y.CompareTo(y.Y));
            double latMin = sortedList[0].Y;
            double latMax = sortedList[^1].Y;

            var coordsLimits = new CoordsLimits
            {
                LonMin = lonMin,
                LonMax = lonMax,
                LatMin = latMin,
                LatMax = latMax
            };

            return coordsLimits;
        }


        public static Coord[] Set_CoordsMatrix(Coord[] coordsArray, double stepDeg, double buffer = double.NaN)
        {
            List<Coord> mtxPoints = new();

            // Get contour latlon limits
            var cntrLim = Extract_CoordsLimits(coordsArray);


            // Set longitude and latitude limits in radians
            var lonStart = Math.Floor(cntrLim.LonMin);
            var lonStop = Math.Ceiling(cntrLim.LonMax);
            var latStart = Math.Floor(cntrLim.LatMin);
            var latStop = Math.Ceiling(cntrLim.LatMax);


            // Generate latitude and longitude range 
            var lonRange = Utils.Arange(lonStart, lonStop, stepDeg).ToArray();
            var latRange = Utils.Arange(latStart, latStop, stepDeg).ToArray();


            // Fill matrix
            for (int i = 0; i < lonRange.Length; i++)
                for (int j = 0; j < latRange.Length; j++)
                {
                    mtxPoints.Add(new Coord(lonRange[i], latRange[j]));
                }

            // Turn list into array
            return mtxPoints.ToArray();
        }


        public static Coord[] Grid_InPolygon(Coord[] cntrArray, double step, double buffer = double.NaN)
        {
            // Clean coordinates duplicates
            cntrArray = Clean_boundaryDuplicates(cntrArray);


            // Create rectangular grid of coordinates for plate boundary, with step as spacing
            var gridPoints = Set_CoordsMatrix(cntrArray, step, buffer);


            // Filter points within plate boundary
            return PolygonGeometry.Points_InPolygon(cntrArray, gridPoints);
        }


        public static Coord[] Clean_boundaryDuplicates(Coord[] cntrArray)
        {

            // Get differencial function from coordinates
            var diffArray = from i in Enumerable.Range(0, cntrArray.Length - 1)
                            select Math.Abs(cntrArray[i + 1].X - cntrArray[i].X) + Math.Abs(cntrArray[i + 1].Y - cntrArray[i].Y);


            // Retrieve index of items where diff is zero
            var zeroPair = diffArray.Select((element, index) => new KeyValuePair<double, double>(index, element)).Where(x => x.Value.Equals(0)).ToArray();
            var zeroArray = (from value in zeroPair select value.Key).ToArray();


            // Filter duplicate longitude coordinates 
            var coordsPair = cntrArray.Select((element, index) => new KeyValuePair<double, Coord>(index, element)).Where(p => zeroArray.All(p2 => p2 != p.Key - 1)).ToArray();
            var coordsList = (from value in coordsPair select value.Value).ToList();


            // Close boundary by duplicating first value at end of array
            if (coordsList[0].X != coordsList.Last().X || coordsList[0].Y != coordsList.Last().Y)
                coordsList.Add(coordsList[0]);


            // Turn list to array
            return coordsList.ToArray();
        }


        public static Coord[] ConcatenateCoords(List<Coord[]> coordsArrays, double delimiter = double.NaN)
        {
            List<Coord> newCoordsList = new();
            Coord[] delimRow = new Coord[1] {new Coord(delimiter, delimiter)};

            foreach (Coord[] coordsArray in coordsArrays)
            {
                newCoordsList.AddRange(coordsArray);
                newCoordsList.AddRange(delimRow);
            }

            // Remove last unused delimiter row
            newCoordsList.RemoveAt(newCoordsList.Count - 1);
            return newCoordsList.ToArray();
        }

        public static double[,] ToArray(Coord[] coords, double[] z)
        {
            if (coords.Length != z.Length)
                throw new ArgumentException(
                    "Error! " +
                    "Coords and z array must be the same lenght."
                    );

            double[,] array = new double[coords.Length, 3];

            for (int i = 0; i < coords.Length; i++)
            {
                array[i, 0] = coords[i].X;
                array[i, 1] = coords[i].Y;
                array[i, 2] = z[i];
            }
            return array;
        }
    }
}
