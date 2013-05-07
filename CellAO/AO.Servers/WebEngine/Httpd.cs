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

    public static class Httpd
    {
        private static HttpListener requestListener { get; set; }
        private static IPAddress Address { get; set; }
        private static int Port { get; set; }

        public static void Start(string Host, int port)
        {
            Address = IPAddress.Parse(Host);
            Port = port;
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Server Starting...");
                Console.ResetColor();
                
                requestListener = new HttpListener();
                requestListener.Prefixes.Add(string.Format("http://*:{0}/", port));
                requestListener.Start();
               requestListener.BeginGetContext(new AsyncCallback(RequestReceived), requestListener);
            }
            catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Unable to start Server..."); Console.ResetColor(); }
        }

        public static void Stop()
        {
            requestListener.Stop();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Server Stopped.");
            Console.ResetColor();
        }

        /// <summary>
        /// Called when an HTTP request is received.
        /// </summary>
        /// <param name="result">Information about the Async Call</param>
        public static void RequestReceived(IAsyncResult result)
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
                    requestListener.BeginGetContext(new AsyncCallback(RequestReceived), requestListener);
                }
                return;
            }
            // Send the context to our manager
            //ConversationManager csm = new ConversationManager(context);

            // Now let the server start the next request
            requestListener.BeginGetContext(new AsyncCallback(RequestReceived), requestListener);
        }

    }
}
