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

        public void Start(string Host, int port)
        {
            this.Address = IPAddress.Parse(Host);
            this.Port = port;
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Server Starting...");
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
            //ConversationManager csm = new ConversationManager(context);

            // Now let the server start the next request
            this.requestListener.BeginGetContext(new AsyncCallback(this.RequestReceived), this.requestListener);
        }

    }
}
