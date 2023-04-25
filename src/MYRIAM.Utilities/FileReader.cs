using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Cartography;

namespace MYRIAM
{
    class FileReader
    {
        public static double GetValue(string value)
        {
            return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string Get_ItemPath(string itemName, string? currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());

            while (directory != null && !directory.GetFiles(itemName).Any() && !directory.GetDirectories(itemName).Any())
            {
                directory = directory.Parent;
            }
            return Path.Combine(directory.FullName, itemName);
        }


        public static string GetContourPath(string plateLabel)
        {
            string dirDefault = Get_ItemPath("assets");
            string dirPath = Path.Combine(dirDefault, "TextFiles/PlateContours_PresentDay_Matthews2016");
            string filePath = Path.Combine(dirPath, $"{plateLabel}_contourXY_Matthews2016.txt");

            if (!File.Exists(filePath))
                throw new InputErrorException(
                    $"Error in PLT_LABEL entry. " +
                    $"Embeded contour with label {plateLabel} does not exist. " +
                    $"Check the Manual for supported plate labels."
                    );

            return filePath;
        }


        public static double[,] File_toArray(string path, int skipRows=0)
        {
            // Read file as lines (skip first)
            string[] lines = File.ReadAllLines(path);
            lines = lines.Skip(skipRows).ToArray();

            int cols = 0;
            int rows = 0;

            // Find number of colums in data, by finding first data row (excluding header)
            bool searchFirstLine = true;
            int k = 0;
            while (searchFirstLine)
            {
                if (lines[k][0].ToString() != "!")
                {
                    cols = lines[k].Split(new char[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    searchFirstLine = false;
                }
                else
                {
                    k++;
                }
            }

            // Find number of rows in data, by finding last data row (excluding bottom empty rows)
            bool searchBottomEmptyLines = true;
            int q = 0;
            while (searchBottomEmptyLines)
            {
                if (lines[lines.Length - 1 - q].ToString() != "")
                {
                    rows = lines.Length - k - q;
                    searchBottomEmptyLines = false;
                }
                else
                {
                    q++;
                }
            }
            
            var dataArray = new double[rows, cols];


            // Iterate over lines in file
            for (int i = k; i < lines.Length - q; i++)
            {
                if (lines[i][0].ToString() != "!")
                {
                    var values = lines[i].Split(new char[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < values.Length; j++)
                        dataArray[i - k, j] = GetValue(values[j]);
                }
            }
            return dataArray;
        }


        public static List<Coordinate[]> File_CoordinatesArray(string args)
        {
            // Read file as lines
            string[] lines = System.IO.File.ReadAllLines(args);


            // Set up empty List
            List<Coordinate[]> polygonList = new();
            List<Coordinate> polygonCoords = new();

            foreach (var line in lines)
            {
                if (line[0].ToString() != "!")
                {
                    var values = line.Split(new char[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    // Store longitude and latitude
                    var lon = GetValue(values[0]);
                    var lat = GetValue(values[1]);

                    if (double.IsNaN(lon))
                    {
                        polygonList.Add(polygonCoords.ToArray());
                        polygonCoords = new List<Coordinate>();
                    }
                    else
                        polygonCoords.Add(new Coordinate() { Lon = lon, Lat = lat});
                }
            }

            // If coordinates file dit not contain nan splitters,
            if (polygonList.Count == 0)

                // Append collected coords to polygonList
                polygonList.Add(polygonCoords.ToArray());


            return polygonList;
        }


        public static Dictionary<string, string> File_toInputStrings(string args)
        {
            // Set output dictionary
            var inputVars = new Dictionary<string, string>();

            // Read file as lines
            string[] lines = File.ReadAllLines(args);

            for (int i = 0; i < lines.Length; i++)
            {
                //string stripLine = Regex.Replace(lines[i], @"^\s+$[\r\n?<>:\|*]", string.Empty, RegexOptions.Multiline);
                string stripLine = String.Join("", lines[i].Split('?', '>', '<', '[', ']', '\"', '\r', '\n', '^', '$', '*', '|', '+'));

                if (stripLine.Length != 0)
                {
                    if (stripLine[0].ToString() != "!")
                    {
                        var entries = stripLine.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        string key, value;

                        if (entries.Length > 2)
                            throw new InputErrorException($"Warning! Too many equals signs in line {i + 1}.");

                        else 
                        {
                            key = entries[0].Trim();

                            if (entries.Length == 1)
                                value = "";
                            
                            else
                                value = entries[1].Trim(); 

                            try
                            {
                                inputVars.Add(key, value);
                            }
                            catch (ArgumentException)
                            {
                                throw new InputErrorException($"Repeated key in input file ({key}).");
                            }
                        }
                    }
                }
            }
            return inputVars;
        }
    }
}
