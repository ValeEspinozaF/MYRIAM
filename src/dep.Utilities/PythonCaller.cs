using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYRIAM
{
    class PythonCaller
    {
        public static void Run_pythonFile(string pythonPath, string pyScriptPath, string args)
        {
            ProcessStartInfo start = new()
            {
                FileName = pythonPath,
                Arguments = string.Format("{0} {1}", pyScriptPath, args),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    //Console.Write(result);
                }
            }
        }
    }
}
