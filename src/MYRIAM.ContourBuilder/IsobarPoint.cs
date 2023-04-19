using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DataStructures;


namespace ContourBuilder
{
    public struct CoordinateXY
    {
        public CoordinateXY(int x, int y)
            : this()
        {
            Y = y;
            X = x;
        }
        public int Y { get; private set; }
        public int X { get; private set; }
    }

    public class IsobarPoint
    { 
        public Isobar? Parent { get; set; }

        public Coord Location { get; set; }

        public double Value { get; set; }

        public CoordinateXY Coordinate { get; set; }

        public IsobarDirection Direction { get; set; }

        public IsobarPoint? FindNext(IsobarPoint[][] vgrid, IsobarPoint[][] hgrid)
        {
            List<IsobarPoint> candidates = new();
            switch (Direction)
            {
                case IsobarDirection.East:
                    if (Coordinate.Y == vgrid[0].Length - 1)
                    {
                        return null;
                    }
                    if (vgrid[Coordinate.X][Coordinate.Y + 1] != null && vgrid[Coordinate.X][Coordinate.Y + 1].Direction == IsobarDirection.East)
                    {
                        candidates.Add(vgrid[Coordinate.X][Coordinate.Y + 1]);
                    }
                    if (Coordinate.Y < hgrid[Coordinate.X].Length)
                    {
                        if (Coordinate.X < hgrid.Length)
                        {
                            if (hgrid[Coordinate.X + 1][Coordinate.Y] != null && hgrid[Coordinate.X + 1][Coordinate.Y].Direction == IsobarDirection.South)
                            {
                                candidates.Add(hgrid[Coordinate.X + 1][Coordinate.Y]);
                            }
                        }
                        if (hgrid[Coordinate.X][Coordinate.Y] != null && hgrid[Coordinate.X][Coordinate.Y].Direction == IsobarDirection.North)
                        {
                            candidates.Add(hgrid[Coordinate.X][Coordinate.Y]);
                        }
                    }
                    break;

                case IsobarDirection.West:
                    if (Coordinate.Y == 0)
                    {
                        return null;
                    }
                    if (vgrid[Coordinate.X][Coordinate.Y - 1] != null && vgrid[Coordinate.X][Coordinate.Y - 1].Direction == IsobarDirection.West)
                    {
                        candidates.Add(vgrid[Coordinate.X][Coordinate.Y - 1]);
                    }
                    if (Coordinate.X < hgrid.Length)
                    {
                        if (hgrid[Coordinate.X + 1][Coordinate.Y - 1] != null && hgrid[Coordinate.X + 1][Coordinate.Y - 1].Direction == IsobarDirection.South)
                        {
                            candidates.Add(hgrid[Coordinate.X + 1][Coordinate.Y - 1]);
                        }
                    }
                    if (hgrid[Coordinate.X][Coordinate.Y - 1] != null && hgrid[Coordinate.X][Coordinate.Y - 1].Direction == IsobarDirection.North)
                    {
                        candidates.Add(hgrid[Coordinate.X][Coordinate.Y - 1]);
                    }
                    break;

                case IsobarDirection.North:
                    if (Coordinate.X == 0)
                    {
                        return null;
                    }
                    if (hgrid[Coordinate.X - 1][Coordinate.Y] != null && hgrid[Coordinate.X - 1][Coordinate.Y].Direction == IsobarDirection.North)
                    {
                        candidates.Add(hgrid[Coordinate.X - 1][Coordinate.Y]);
                    }
                    if (Coordinate.X > 0)
                    {
                        if (vgrid[Coordinate.X - 1][Coordinate.Y] != null && vgrid[Coordinate.X - 1][Coordinate.Y].Direction == IsobarDirection.West)
                        {
                            candidates.Add(vgrid[Coordinate.X - 1][Coordinate.Y]);
                        }
                        if (Coordinate.Y < vgrid[Coordinate.X - 1].Length + 1)
                        {
                            if (vgrid[Coordinate.X - 1][Coordinate.Y + 1] != null  && vgrid[Coordinate.X - 1][Coordinate.Y + 1].Direction == IsobarDirection.East)
                            {
                                candidates.Add(vgrid[Coordinate.X - 1][Coordinate.Y + 1]);
                            }
                        }
                    }
                    break;

                case IsobarDirection.South:
                    if (Coordinate.X == hgrid.Length - 1)
                    {
                        return null;
                    }
                    if (hgrid[Coordinate.X + 1][Coordinate.Y] != null && hgrid[Coordinate.X + 1][Coordinate.Y].Direction == IsobarDirection.South)
                    {
                        candidates.Add(hgrid[Coordinate.X + 1][Coordinate.Y]);
                    }
                    if (Coordinate.X < vgrid.Length)
                    {
                        if (vgrid[Coordinate.X][Coordinate.Y + 1] != null && vgrid[Coordinate.X][Coordinate.Y + 1].Direction == IsobarDirection.East)
                        {
                            candidates.Add(vgrid[Coordinate.X][Coordinate.Y + 1]);
                        }

                        if (vgrid[Coordinate.X][Coordinate.Y] != null && vgrid[Coordinate.X][Coordinate.Y].Direction == IsobarDirection.West)
                        {
                            candidates.Add(vgrid[Coordinate.X][Coordinate.Y]);
                        }
                    }
                    break;
            }
            if (candidates == null)
            {
                return null;
            }
            return candidates.Where(x => x.Parent == null && x.Value == Value).FirstOrDefault();
        }
    }
}