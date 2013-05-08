using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using _config = AO.Core.Config;

namespace WebEngine
{
    public class PhpCheck
    {
        //private string WebServerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName);
        private string msiPath = string.Empty;
        private static FileInfo WebServerInstallPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().FullName);

        private string WebServerPath = WebServerInstallPath.DirectoryName;
            
        public void Check()
        {
            //Checking if user has Php.exe installed.

            if (IsPHPExePresent)
            {
                PhpParser.PHPLocation = Path.Combine(WebServerPath, "php\\php.exe");
                return;
            }
            if (!Directory.Exists(_config.ConfigReadWrite.Instance.CurrentConfig.PhpPath))
            {
                Console.WriteLine("Downloading PHP");
                try
                {
                    Uri phpLink = GetNewestPHPLink();
                    string[] file = phpLink.ToString().Split('/');
                    string fileName = file[file.Length -1];
                    Console.WriteLine(phpLink.ToString());
                    string dir = Path.Combine(WebServerPath, "temp");
                    Console.WriteLine(dir);
                    Directory.CreateDirectory(dir);
                    WebClient wc = new WebClient();
                    msiPath = Path.Combine(dir, fileName);
                    wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    wc.DownloadFileAsync(phpLink, msiPath);//WebServerPath + "\\temp\\php-5.3.24-nts-Win32-VC9-x86.msi");

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
        private void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.Write("\rCompleted");
            Console.WriteLine();
            Console.WriteLine("Installing php");
            InstallPhP(msiPath);
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\rDownloading: {0}", e.ProgressPercentage + "%");
        }
        private Uri GetNewestPHPLink()
        {
            Uri newLink = null;
            // This is the releases page, the newest version will be the most recent date
            Uri releases = new Uri("http://windows.php.net/downloads/releases/");
            try
            {
                // get the page
                string html = string.Empty;
                using (WebClient wc = new WebClient())
                {
                    html = wc.DownloadString(releases);
                }

                // get first msi
                int index = html.IndexOf("msi");
                string cut = html.Substring(0, index) + "msi";
                string[] all = cut.Split(' ');
                string end = all[all.Length - 1].Remove(0, 7);
                newLink = new Uri("http://windows.php.net/" + end);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return newLink;
        }
        private void InstallPhP(string msiLoc)
        {
            // Install dir
            string installDir = Path.Combine(WebServerPath, "php");

            try
            {
                ProcessStartInfo php = new ProcessStartInfo("msiexec.exe");
                php.UseShellExecute = false;
                php.Arguments = "/i "+msiLoc+" /qn ADDLOCAL=cgi,ext_php_mysqli INSTALLDIR=\u0022" + installDir+"\u0022";
                Console.WriteLine(php.Arguments);
                php.CreateNoWindow = true;
                Process p = Process.Start(php);
                p.WaitForExit();
                PhpParser.PHPLocation = Path.Combine(WebServerPath, "php\\php.exe");
                Console.WriteLine("Install Complete");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private bool IsPHPExePresent
        {
            get { return File.Exists(Path.Combine(WebServerPath, "php\\php.exe")); }
        }
    }
}
