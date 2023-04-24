﻿using DataStructures;
using EnsembleAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms;
using Cartography;


namespace ContourBuilder
{
    public class Contour
    {
        public Contour(List<Isobar> isoBars, double level, double percentage)
        {
            Length = isoBars.Count;
            Level = level;
            PercentInterval = percentage;

            var CoordinatesList = new List<Coordinate>();
            foreach (var isobar in isoBars)
            {
                foreach (IsobarPoint point in isobar.Points)
                {
                    CoordinatesList.Add(new Coordinate(point.Location.Lon, point.Location.Lat));
                }
                CoordinatesList.Add(new Coordinate(double.NaN, double.NaN));
            }

            Coordinates = CoordinatesList.ToArray();
            Coordinates = Coordinates.Take(Coordinates.Length - 1).ToArray();
        }

        public int Length { get; private set; }
        public double Level { get; private set; }
        public double PercentInterval { get; private set; }
        public Coordinate[] Coordinates { get; set; }


        public static Contour[] CreateContour(Histogram2D hist2D, double[] confPercent)
        {
            Contour[] contours = new Contour[confPercent.Length];

            // Determine level for each percentage tolerance
            double[] levels = Ensemble_Statistics.Extract_ConfidenceLevels2D(hist2D.Values, confPercent);

            for (int i = 0; i < confPercent.Length; i++)
            {
                // Find dataset's level for given confidence invertal (in percentage)
                var isoBars = Isobar.CreateIsobar(hist2D, levels[i]);

                // Store data as Contour structure
                contours[i] = new Contour(isoBars, levels[i], confPercent[i]);
            }

            return contours;
        }
    }
}
