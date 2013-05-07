using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

using _config = AO.Core.Config;

namespace WebEngine
{
    public class ConverstationManager
    {

        private string WWWRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\htdocs\";
        private HttpListenerContext Context { get; set; }

        private StreamReader Reader { get; set; }
        private StreamWriter Writer { get; set; }

        public ConverstationManager(HttpListenerContext context)
        {
            this.Context = context;
            Console.WriteLine("Request was for {0}", context.Request.RawUrl);
            Writer = new StreamWriter(context.Response.OutputStream);
            Reader = new StreamReader(context.Request.InputStream);
            string reply = ProccessURL(context.Request.RawUrl);

            Writer.Write(reply);
            //Writer.Flush();
            Writer.Close();
            Context.Response.Close();
        }

        public string ProccessURL(string request)
        {
            string result = string.Empty;
           // if (request.EndsWith(".php")) { ParsePHP(request); }
            if (request.EndsWith(".php")) { PhpPharser.ParsePhpFile(_config.ConfigReadWrite.Instance.CurrentConfig.PhpPath + @"\Php.exe", request, out result); }
            else
                switch (request)
                {
                    case "/":
                        result = File.ReadAllText(Path.Combine(WWWRoot, "index.html"));
                        break;
                    default:
                        {
                            try
                            {
                                result = File.ReadAllText(Path.Combine(WWWRoot, request.Substring(1)));
                            }
                            catch (Exception)
                            {
                                result = string.Format("404 Error File {0} not found", Path.Combine(WWWRoot, request));
                            }
                        }
                        break;
                }
            return result;
        }
    }
}
