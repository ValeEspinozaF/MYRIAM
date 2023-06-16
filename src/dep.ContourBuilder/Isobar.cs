using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Histogram;
using Cartography;
using DataStructures;

// Credits to Uri Agassi for the base code (https://github.com/uriagassi/Creating-Isobars/).
namespace ContourBuilder
{
    public enum IsobarDirection
    {
        North,
        South,
        East,
        West,
    }

    public class Isobar
    {
        public Isobar(IsobarPoint first, IsobarPoint[][] vgrid, IsobarPoint[][] hgrid, bool isClosed)
        {
            Value = first.Value;
            IsClosed = isClosed;
            Points = GetPoints(first, vgrid, hgrid).ToArray();
        }


        private IEnumerable<IsobarPoint> GetPoints(IsobarPoint first, IsobarPoint[][] vgrid, IsobarPoint[][] hgrid) 
        {
            first.Parent = this;
            yield return first;
            IsobarPoint next, current = first;
            while ((next = current.FindNext(vgrid, hgrid)) != null && next != first)
            {
                next.Parent = this;
                yield return next;
                current = next;
            }
        }


        public IsobarPoint[] Points { get; private set; }

        public double Value { get; private set; }

        public bool IsClosed { get; private set; }

        public static List<Isobar> CreateIsobar(Histogram2D hist2D, double level)
        {
            var bins = hist2D.BinsAndValues;

            int xLength = hist2D.BinsAndValues.GetLength(0);
            int yLength = hist2D.BinsAndValues.GetLength(1);

            IsobarPoint[][] hgrid = new IsobarPoint[xLength][];
            IsobarPoint[][] vgrid = new IsobarPoint[xLength - 1][];

            for (int i = 0; i < xLength; i++)
            {
                if (i != xLength - 1)
                {
                    vgrid[i] = new IsobarPoint[yLength];
                }

                hgrid[i] = new IsobarPoint[yLength - 1];
                for (int j = 0; j < yLength; j++)
                {
                    var bin = bins[i, j];
                    var value = bin.Value;

                    if (i != 0)
                    {
                        if ((value <= level && level < bins[i - 1, j].Value) || (value >= level && level > bins[i - 1, j].Value))
                        {
                            double ratio = (level - value) / (bins[i - 1, j].Value - value);
                            double x = bin.Key.MidCoordinates.X;
                            double y = (bins[i - 1, j].Key.MidCoordinates.Y - bin.Key.MidCoordinates.Y) * ratio + bin.Key.MidCoordinates.Y;

                            vgrid[i - 1][j] = new IsobarPoint
                                {
                                    Coordinate = new CoordinateXY(i - 1, j),
                                    Location = Coordinate.FromDegrees(x, y),
                                    Direction = (value > bins[i - 1, j].Value) ? IsobarDirection.East : IsobarDirection.West, // circle the mountain clockwise
                                Value = level
                                };
                            
                        }
                    }
                    if (j < yLength - 1)
                    {
                        if ((value < level && level <= bins[i, j + 1].Value) || (value > level && level >= bins[i, j + 1].Value))
                        {
                            double ratio = (level - value) / (bins[i, j + 1].Value - value);
                            double x = (bins[i, j + 1].Key.MidCoordinates.X - bin.Key.MidCoordinates.X) * ratio + bin.Key.MidCoordinates.X;
                            double y = bin.Key.MidCoordinates.Y; 

                            hgrid[i][j] = new IsobarPoint
                            {
                                Coordinate = new CoordinateXY(i, j),
                                Location = Coordinate.FromDegrees(x, y),
                                Direction = (value > bins[i, j + 1].Value) ? IsobarDirection.South : IsobarDirection.North,
                                Value = level
                            };
                        }
                    }
                }
            }
            return GenerateIsobars(vgrid, hgrid);;
        }


        private static List<Isobar> GenerateIsobars(IsobarPoint[][] vgrid, IsobarPoint[][] hgrid)
        {
            List<Isobar> isoBarList = new List<Isobar>();

            // Find open contours (border un-closed isobars)
            foreach (var l in vgrid)
            {
                if(l[0] != null && l[0].Direction == IsobarDirection.East)
                {
                    isoBarList.Add(new Isobar(l[0], vgrid, hgrid, false));
                }
                if (l.Last() != null && l.Last().Direction == IsobarDirection.West)
                {
                    isoBarList.Add(new Isobar(l.Last(), vgrid, hgrid, false));
                }
            }
            foreach (var i in hgrid[0])
            {
                if (i != null && i.Direction == IsobarDirection.South)
                {
                    isoBarList.Add(new Isobar(i, vgrid, hgrid, false));
                }
            }
            foreach (var i in hgrid.Last())
            {
                if (i != null && i.Direction == IsobarDirection.North)
                {
                    isoBarList.Add(new Isobar(i, vgrid, hgrid, false));
                }
            }

            // Find closed contours
            for (int y = 1; y < vgrid.Length - 1; y++)
            {
                for (int x = 1; x < vgrid[y].Length - 1; x++)
                {
                    if (vgrid[y][x] != null && vgrid[y][x].Parent == null)
                    {
                        isoBarList.Add(new Isobar(vgrid[y][x], vgrid, hgrid, true));
                    }
                }
            }
            return isoBarList;
        }
    }
}
