using Cartography;


namespace CartographicCoordinates
{
    class ManageCoordinates
    {
        public static Coordinate[] Grid_InPolygon(Coordinate[] cntrArray, double step)
        {
            // Clean coordinates duplicates
            cntrArray = Clean_boundaryDuplicates(cntrArray);


            // Create rectangular grid of coordinates for plate boundary, with step as spacing
            var gridPoints = Coordinate.MakeGrid(cntrArray, step);


            // Filter points within plate boundary
            return PolygonGeometry.Points_InPolygon(cntrArray, gridPoints);
        }

        public static Coordinate[] Grid_InPolygon(List<Coordinate[]> cntrArrays, double stepDeg)
        {
            // Create grid of coordinates within multiple plate boundary limits
            // with stepDeg as spacing
            Coordinate[] gridPntsDeg = cntrArrays.
                SelectMany(coordArray => Grid_InPolygon(coordArray, stepDeg)).
                ToArray();


            // Eliminate grid duplicates (same latitude, one with Lon=180, the other Lon=-180)
            gridPntsDeg = gridPntsDeg.
                Where(x => !(x.Lon == -180 && gridPntsDeg.Where(p => p.Lon == 180).Select(p => p.Lat).Contains(x.Lat))).
                ToArray();


            // Return grid points in degrees
            return gridPntsDeg;
        }

        public static Coordinate[] Clean_boundaryDuplicates(Coordinate[] cntrArray)
        {

            // Get differencial function from coordinates
            var diffArray = from i in Enumerable.Range(0, cntrArray.Length - 1)
                            select Math.Abs(cntrArray[i + 1].Lon - cntrArray[i].Lon) + Math.Abs(cntrArray[i + 1].Lat - cntrArray[i].Lat);


            // Retrieve index of items where diff is zero
            var zeroPair = diffArray.Select((element, index) => new KeyValuePair<double, double>(index, element)).Where(x => x.Value.Equals(0)).ToArray();
            var zeroArray = (from value in zeroPair select value.Key).ToArray();


            // Filter duplicate longitude coordinates 
            var coordsPair = cntrArray.Select((element, index) => new KeyValuePair<double, Coordinate>(index, element)).Where(p => zeroArray.All(p2 => p2 != p.Key - 1)).ToArray();
            var coordsList = (from value in coordsPair select value.Value).ToList();


            // Close boundary by duplicating first value at end of array
            if (coordsList[0].Lon != coordsList.Last().Lon || coordsList[0].Lat != coordsList.Last().Lat)
                coordsList.Add(coordsList[0]);


            // Turn list to array
            return coordsList.ToArray();
        }


        public static Coordinate[] ConcatenateCoords(List<Coordinate[]> coordsArrays, double delimiter = double.NaN)
        {
            List<Coordinate> newCoordsList = new();
            Coordinate[] delimRow = new Coordinate[1] {new Coordinate(delimiter, delimiter)};

            foreach (Coordinate[] coordsArray in coordsArrays)
            {
                newCoordsList.AddRange(coordsArray);
                newCoordsList.AddRange(delimRow);
            }

            // Remove last unused delimiter row
            newCoordsList.RemoveAt(newCoordsList.Count - 1);
            return newCoordsList.ToArray();
        }
    }
}
