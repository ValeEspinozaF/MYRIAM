using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections;
using static MYRIAM.ManageOutputs;
using System.Xml.Linq;
using DataStructures;


namespace MYRIAM
{
    internal class Console_Banners
    {
        const char _block = '■';
        const string _twirl = "-\\|/";

        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor); 
        }

        private static void ReplacePreviousConsoleLine(string newLine)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor -1);
            Console.WriteLine(newLine);
            Console.SetCursorPosition(0, currentLineCursor);
        }

        internal static void MainBanner()
        {
            Console.WriteLine("");
            Console.WriteLine("===================================================================");
            Console.WriteLine("|                             MYRIAM                              |");
            Console.WriteLine("|                                                                 |");
            Console.WriteLine("|  MYRIAM is an open-source software that implements tomography-  |"); 
            Console.WriteLine("|  based lateral variations on the viscous shear-resistance to    |");
            Console.WriteLine("|  calculate the torque variation required to produce a given     |");
            Console.WriteLine("|  change in plate motion.                                        |");
            Console.WriteLine("|                                                                 |");
            Console.WriteLine("|  Valentina Espinoza                                             |");
            Console.WriteLine("|  Juan Martin de Blas                                            |");
            Console.WriteLine("|  Giampiero Iaffaldano                                           |");
            Console.WriteLine("|                                                                 |");
            Console.WriteLine("|  Copenhagen, 2023                                               |");
            Console.WriteLine("===================================================================");
            Console.WriteLine("");
        }


        internal static void InputSummaryBanner(InputParameters inputParams)
        {
            string str_muA = SetString(inputParams.muA, "#.#E+0");
            string str_muM = SetString(inputParams.muM, "#.#E+0");
            string str_HL  = SetString(inputParams.HL, "F0");
            string str_fHA = SetString(inputParams.FRACTION_HA, "F2");

            ClearCurrentConsoleLine();
            ReplacePreviousConsoleLine("Input Summary");
            Console.WriteLine("");
            Console.WriteLine(String.Format("{0, 34} {1}", "Plate :", inputParams.PLT_LABEL));
            Console.WriteLine(String.Format("{0, 34} {1} Pa s", "muA :", str_muA));
            Console.WriteLine(String.Format("{0, 34} {1} Pa s", "muM :", str_muM));
            Console.WriteLine(String.Format("{0, 34} {1} km", "LAB's Depth :", str_HL));
            Console.WriteLine(String.Format("{0, 34} {1} ", "Asthenosphere fraction used :", str_fHA));
            Console.WriteLine("\n");
        }


        internal static void OutputSummaryBanner(TorqueVector dMvector)
        {
            ClearCurrentConsoleLine();
            ReplacePreviousConsoleLine("Output Torque Variation (dM) Summary");
            Console.WriteLine("");

            // Average Pole
            Console.WriteLine(String.Format("{0, 34} {1}", "Average Pole :", 
                $"{SetString(dMvector.Longitude, "F2")} °E, " +
                $"{SetString(dMvector.Latitude, "F2")} °N")
                );

            // Average Magnitude
            Console.WriteLine(String.Format("{0, 34} {1}", "Average Magnitude :", 
                $"{SetString(dMvector.Magnitude, "#.##E+0")} N m")
                );

            // Average Cartesian Vector
            Console.WriteLine(String.Format("{0, 34} {1}", "Average Cartesian Vector :", 
                $"{SetString(dMvector.X, "#.##E+0")}, " +
                $"{SetString(dMvector.Y, "#.##E+0")}, " +
                $"{SetString(dMvector.Z, "#.##E+0")}, ")
                );
            Console.WriteLine("\n");
        }

        public static void WriteReports(int reportNr, int iteration = 0, int itLength = 1)
        {
            switch (reportNr)
            {
                case 1:
                    {
                        Console.WriteLine("Managing input parameters");
                        WriteProgressBar(0, false);
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Creating output directories");
                        WriteProgressBar(5, false);
                        break;
                    }
                case 3:
                    {
                        ReplacePreviousConsoleLine("Generating plate's base grid");
                        WriteProgressBar(6, true);
                        break;
                    }
                case 4:
                    {
                        ReplacePreviousConsoleLine("Calculating asthenosphere viscosity distribution");
                        WriteProgressBar(10, true);
                        break;
                    }
                case 5:
                    {
                        int progress = 10 + (int)Math.Round((15.0 / itLength) * (iteration + 1));
                        WriteProgressBar(progress, true);
                        break;
                    }
                case 6:
                    {
                        ReplacePreviousConsoleLine("Interpolating plate's grid viscosity");
                        WriteProgressBar(30, false);
                        break;
                    }
                case 7:
                    {
                        ReplacePreviousConsoleLine("Build operator w2M");
                        WriteProgressBar(31, true);
                        break;
                    }
                case 8:
                    {
                        ReplacePreviousConsoleLine("Save REPOSITORY_w2M text files");
                        WriteProgressBar(34, true);
                        break;
                    }
                case 9:
                    {
                        ReplacePreviousConsoleLine("Load/sample Euler vector stages");
                        for (var i = 1; i <= 6; ++i)
                        {
                            WriteProgressBar(34 + i, true);
                            Thread.Sleep(200);
                        }
                        WriteProgressBar(40, true);
                        break;
                    }
                case 10:
                    {
                        ReplacePreviousConsoleLine("Calculate torque variation");
                        WriteProgressBar(45, true);
                        break;
                    }
                case 11:
                    {
                        ReplacePreviousConsoleLine("Save torque variation text files");
                        WriteProgressBar(50, true);
                        break;
                    }
                case 12:
                    {
                        if (iteration == 0)
                        {
                            Console.WriteLine("Skip magnitude histogram");
                            WriteProgressBar(51);
                        }

                        else if (iteration == 1)
                        {
                            Console.WriteLine("Calculate magnitude histogram");
                            WriteProgressBar(51);
                        }

                        for (var i = 2; i <= 5; ++i)
                        {
                            WriteProgressBar(50 + i, true);
                            Thread.Sleep(5);
                        }

                        break;
                    }
                case 13:
                    {
                        int _case = iteration;
                        int _subcase = itLength;

                        if (_case == 0)
                        {
                            ReplacePreviousConsoleLine("Skip pole contouring");
                            for (var i = 1; i <= 15; ++i)
                            {
                                WriteProgressBar(55 + i, true);
                                Thread.Sleep(5);
                            }
                        }
                        else if (_case == 1)
                        {
                            if (_subcase == 0)
                            {
                                ReplacePreviousConsoleLine("Calculate pole contours");
                            }
                            else if (_subcase == 1)
                                for (var i = 1; i <= 15; ++i)
                                {
                                    WriteProgressBar(55 + i, true);
                                    Thread.Sleep(40);
                                }
                        }
                        break;
                    }
                case 14:
                    {
                        bool _do = iteration != 0;

                        if (_do == true)
                        {
                            ReplacePreviousConsoleLine("Make Figures");
                        }
                        else
                        {
                            ReplacePreviousConsoleLine("Skip figure making");
                            for (var i = 1; i <= 30; ++i)
                            {
                                WriteProgressBar(70 + i, true);
                                Thread.Sleep(5);
                            }
                        }

                        break;
                    }
                case 15:
                    {
                        int _case = iteration;
                        bool _do = itLength != 0;

                        // Grid Maps
                        if (_case == 0)
                            for (var i = 1; i <= 10; ++i)
                            {
                                ReplacePreviousConsoleLine("Make Figures - Grid Maps");
                                WriteProgressBar(70 + i, true);
                                Thread.Sleep(300);
                            }

                        // Magnitude Histogram1D
                        else if (_case == 1)
                            if (_do == true)
                            {
                                for (var i = 1; i <= 5; ++i)
                                {
                                    ReplacePreviousConsoleLine("Make Figures - Magnitude Histogram1D");
                                    WriteProgressBar(80 + i, true);
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                for (var i = 1; i <= 5; ++i)
                                {
                                    ReplacePreviousConsoleLine("Make Figures - Skip Magnitude Histogram1D");
                                    WriteProgressBar(80 + i, true);
                                    Thread.Sleep(5);
                                }
                            }


                        // Contour Map
                        else if (_case == 2)
                            if (_do == true)
                            {
                                for (var i = 1; i <= 5; ++i)
                                {
                                    ReplacePreviousConsoleLine("Make Figures - Pole Contour Map");
                                    WriteProgressBar(85 + i, true);
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                for (var i = 1; i <= 15; ++i)
                                {
                                    ReplacePreviousConsoleLine("Make Figures - Skip Pole Maps");
                                    WriteProgressBar(85 + i, true);
                                    Thread.Sleep(5);
                                }
                            }

                        // Rotated ensemble map
                        else if (_case == 3)
                            if (_do == true)
                            {
                                for (var i = 1; i <= 10; ++i)
                                {
                                    ReplacePreviousConsoleLine("Make Figures - Rotated Ensemble Map");
                                    WriteProgressBar(90 + i, true);
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                for (var i = 1; i <= 10; ++i)
                                {
                                    ReplacePreviousConsoleLine("Make Figures - Skip Rotated Ensemble Map");
                                    WriteProgressBar(90 + i, true);
                                    Thread.Sleep(5);
                                }
                            }

                        break;
                    }
                case 16:
                    {
                        ReplacePreviousConsoleLine("Done!");
                        WriteProgressBar(100, true);
                        Console.WriteLine("\n");
                        break;
                    }
            }
        }

        public static void WriteProgressBar(int percent, bool update = false)
        {
            //if(update)
            //             ClearCurrentConsoleLine();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("[");
			var p = (int)((percent * 3f/ 5f) + .5f);
			for (var i = 0; i<60; i++)
			{
				if (i >= p)
					Console.Write(' ');
				else
					Console.Write(_block);
			}
            Console.Write("] {0,3:##0}%", percent);
        }
        public static void WriteProgress(int progress, bool update = false)
        {
            if (update)
                Console.Write("\b");

            Console.Write(_twirl[progress % _twirl.Length]);
        }
    }
}
