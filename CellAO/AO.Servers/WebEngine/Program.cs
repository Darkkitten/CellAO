#region License
/*
Copyright (c) 2005-2013, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NBug;
using NBug.Properties;

using NLog;
using NLog.Config;
using NLog.Targets;

using AO.Core;
using _config = AO.Core.Config;

namespace WebEngine
{
    class Program
    {
        private HttpListener requestListener { get; set; }
        private IPAddress Address { get; set; }
        private int Port { get; set; }

        static void Main(string[] args)
        {

            #region Console base Text
            Console.Title = "CellAO WebEgine" + AssemblyInfoclass.Title + " Console. Version: " + AssemblyInfoclass.Description
    + " " + AssemblyInfoclass.AssemblyVersion + " " + AssemblyInfoclass.Trademark;

            ConsoleText ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.WriteLine("Loading " + AssemblyInfoclass.Title + "...");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
            Console.ResetColor();
            #endregion

            #region Php.exe Check..?
            PhpCheck phpc = new PhpCheck();
            //Code goes here to execute the Class for PHP Check and download / Install etc.
            #endregion

            bool processedargs = false;

            #region NLog
            LoggingConfiguration config = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/WebEngineLog.txt";
            fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);
            LogManager.Configuration = config;
            #endregion

            #region NBug
            SettingsOverride.LoadCustomSettings("NBug.WebEngine.Config");
            NBug.Settings.WriteLogToDisk = true;
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;
            //TODO: ADD More Handlers.
            #endregion

            #region Console Commands
            string consoleCommand;
            #region Locale
            if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "en")
            {
                ct.TextRead("en_web_consolecommands.txt");
            }
            else if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "ro")
            {
                ct.TextRead("ro_web_consolecommands.txt");
            }
            else if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "gr")
            {
                ct.TextRead("ro_web_consolecommands.txt");
            }
            else if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "ee")
            {
                ct.TextRead("ee_web_consolecommands.txt");
            }
            #endregion
            while (true)
            {
                if (!processedargs)
                {
                    if (args.Length == 1)
                    {
                        if (args[0].ToLower() == "/autostart")
                        {
                            ct.TextRead("autostart.txt");
                            Httpd.Start(_config.ConfigReadWrite.Instance.CurrentConfig.WebHost, _config.ConfigReadWrite.Instance.CurrentConfig.WebPort);
                        }
                    }
                    processedargs = true;
                }
                Console.Write("\nServer Command >>");

                consoleCommand = Console.ReadLine();
                string temp = "";
                while (temp != consoleCommand)
                {
                    temp = consoleCommand;
                    consoleCommand = consoleCommand.Replace("  ", " ");
                }
                consoleCommand = consoleCommand.Trim();
                switch (consoleCommand.ToLower())
                {
                    case "start":
                        Httpd.Start(_config.ConfigReadWrite.Instance.CurrentConfig.WebHost, _config.ConfigReadWrite.Instance.CurrentConfig.WebPort);
                        break;
                    case "stop":
                        Httpd.Stop();
                        break;
                    case "exit":
                        Process.GetCurrentProcess().Kill();
                        break;
                    default:
                        if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "en")
                            ct.TextRead("en_web_consolecmddefault.txt");
                        else if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "ro")
                            ct.TextRead("ro_web_consolecmddefault.txt");
                        else if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "gr")
                            ct.TextRead("gr_web_consolecmddefault.txt");
                        else if (_config.ConfigReadWrite.Instance.CurrentConfig.Locale == "ee")
                            ct.TextRead("ee_web_consolecmddefault.txt");
                        break;
                }
            #endregion
            }
        }
    }
}
