using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace WebEngine
{
    public class Httpd
    {
        private HttpListener requestListener { get; set; }
        private IPAddress Address { get; set; }
        private int Port { get; set; }

        /// <summary>
        /// Starts our Httpd server
        /// </summary>
        /// <param name="Host">must be in dotted ip form IE: 127.0.0.1</param>
        /// <param name="port"></param>
        public void Start(string Host, int port)
        {
            IPAddress address = IPAddress.Any;
            if (IPAddress.TryParse(Host, out address)) { this.Address = address; }
            else // Must be a hostname
            { this.Address = ConvertFromHostName(Host); }

            this.Port = port;
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Server Starting on Host {0}, Port {1}", Host, port);
                Console.ResetColor();
                
                this.requestListener = new HttpListener();
                this.requestListener.Prefixes.Add(string.Format("http://*:{0}/", port));
                this.requestListener.Start();
                this.requestListener.BeginGetContext(new AsyncCallback(this.RequestReceived), this.requestListener);
            }
            catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Unable to start Server..."); Console.ResetColor(); }
        }

        public void Stop()
        {
            this.requestListener.Stop();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Server Stopped.");
            Console.ResetColor();
        }

        /// <summary>
        /// Called when an HTTP request is received.
        /// </summary>
        /// <param name="result">Information about the Async Call</param>
        public void RequestReceived(IAsyncResult result)
        {
            // Retrieve the object that called the Asynch Operation
            HttpListener baseListener = (HttpListener)result.AsyncState;

            HttpListenerContext context = null;
            try
            {
                // Complete the operation (let the incoming request finish requesting)
                context = baseListener.EndGetContext(result);
            }
            catch (HttpListenerException e)
            {
                Console.WriteLine(e.Message);
                if (requestListener.IsListening)
                {
                    this.requestListener.BeginGetContext(new AsyncCallback(this.RequestReceived), this.requestListener);
                }
                return;
            }
            // Send the context to our manager
            ConverstationManager csm = new ConverstationManager(context);

            // Now let the server start the next request
            this.requestListener.BeginGetContext(new AsyncCallback(this.RequestReceived), this.requestListener);
        }
        /// <summary>
        /// Attempts to convert from a hostname to a IPAddress
        /// </summary>
        /// <param name="hostname">the hostname</param>
        /// <returns>hostname converted to IPAddress, or IPAddress.Any</returns>
        public IPAddress ConvertFromHostName(string hostname)
        {
            IPAddress[] addresslist = Dns.GetHostAddresses(hostname);
            IPAddress result = IPAddress.Any;
            foreach (IPAddress theaddress in addresslist)
            {
                if (theaddress.ToString() == hostname)
                    result = theaddress;
            }
            return result;
        }
    }
}
