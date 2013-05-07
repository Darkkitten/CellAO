using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace WebEngine
{
    public static class PhpPharser
    {
        /// <summary>
        /// This is our Php Pharser
        /// </summary>
        /// <param name="phpExeLoc">Location of Php</param>
        /// <param name="fileToParseLoc">Php File to Parse htdocs default.</param>
        /// <param name="parsedFile"> File to save Parsed document to.</param>
        public static void ParsePhpFile(string phpExeLoc, string fileToParseLoc, out string parsedFile)
        {
            parsedFile = string.Empty;
            // Runs a file through our php interpreter
            if (!File.Exists(phpExeLoc) || !File.Exists(fileToParseLoc)) { return; }
            FileInfo phpInfo = new FileInfo(phpExeLoc);
            ProcessStartInfo php = new ProcessStartInfo(phpExeLoc);
            php.Arguments = fileToParseLoc;
            php.CreateNoWindow = true;
            php.RedirectStandardOutput = true;
            php.UseShellExecute = false;
            php.WorkingDirectory = phpInfo.DirectoryName;

            try
            {
                using (Process pro = Process.Start(php))
                {
                    using (StreamReader reader = pro.StandardOutput)
                    {
                        parsedFile = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }
    }
}
