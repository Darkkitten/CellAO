using System;
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
        public void Check()
        {
            //Checking if user has Php.exe installed.
            string WebServerPath =  Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!Directory.Exists(_config.ConfigReadWrite.Instance.CurrentConfig.PhpPath))
            {
                Console.WriteLine("Downloading PHP");
                try
                {
                    Directory.CreateDirectory(WebServerPath+"\\temp");
                    WebClient wc = new WebClient();
                  //  wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
                   // wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    //wc.DownloadFileAsync("http://windows.php.net/downloads/releases/php-5.3.24-nts-Win32-VC9-x86.msi", WebServerPath + "\\temp\\php-5.3.24-nts-Win32-VC9-x86.msi");
                }
                catch {}
            }
        }
    }
}
