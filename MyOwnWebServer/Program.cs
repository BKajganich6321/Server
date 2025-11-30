using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnWebServer
{
    internal class Program
    {
        private static string WebRoot {  get; set; }        
        static async Task Main(string[] args)
        {
            // Validate Args
            if (Validator.CheckArgs(args))
            {
                // Invalid arguments, exit application
                return;
            }

            // Args are valid
            WebRoot = args[0].Split('=')[1];
            IPAddress webIP = IPAddress.Parse(args[1].Split('=')[1]);
            Int32 webPort = Int32.Parse(args[2].Split('=')[1]);

            // Start listener
            TcpListener listener = new TcpListener(webIP, webPort);
            RespondForever(listener);
        }

        private static async void RespondForever(TcpListener listener)
        {
            // Listen for requests forever
            bool isRunning = true;
            listener.Start();
            while (isRunning)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                string requestMessage;
                string responseMessage;
                try
                {
                    requestMessage = GetRequest(stream);
                    responseMessage = GetResponse(requestMessage);
                }
                catch
                {
                    responseMessage = "HTTP/1.1 500 Internal Server Error";
                }
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        private static string GetRequest(NetworkStream clientStream)
        {

            byte[] data = new byte[1024];

            string fullRequest = "";

            int numBytesRead;
            while ((numBytesRead = clientStream.Read(data, 0, data.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                fullRequest += System.Text.Encoding.ASCII.GetString(data, 0, numBytesRead);
            }

            return fullRequest;
        }


        private static string GetResponse(string request)
        {
            int statusCode = ValidateGetRequest(request);
            if (statusCode == 200)
            {
                return "HTTP/1.1 200 OK";
            }
            else
            {
                return "HTTP/1.1 " + statusCode + " " + ConvertStatusCodeToString(statusCode);
            }
        }

        private static string ConvertStatusCodeToString(int statusCode)
        {
            string result = "Unknown";
            if (statusCode == 200)
            {
                result = "OK";
            }
            else if (statusCode == 400)
            {
                result = "Bad Request";
            }
            else if (statusCode == 404)
            {
                result = "Not Found";
            }
            else if (statusCode == 405)
            {
                result = "Method Not Allowed";
            }
            else if (statusCode == 505)
            {
                result = "HTTP Version Not Supported";
            }
            else if (statusCode == 505)
            {
                result = "HTTP Version Not Supported";
            }
            return result;
        }

        private static int ValidateGetRequest(string request)
        {
            string[] lines = request.Split('\n');
            string[] firstLineArray = lines[0].Split(' ');

            int statusCode = 200;

            try
            {
                if (lines.Length < 2)
                {
                    statusCode = 400;
                }
                else if (firstLineArray.Length != 3)
                {
                    statusCode = 400;
                }
                else if (firstLineArray[0].Trim() != "GET")
                {
                    statusCode = 405;
                }
                else if (!File.Exists(WebRoot + firstLineArray[1].Trim()))
                {
                    statusCode = 404;
                }
                else if (firstLineArray[2].Trim() != "HTTP/1.1")
                {
                    statusCode = 505;
                }

                string[] secondLineArray = lines[1].Split(':');
                if (secondLineArray.Length != 2)
                {
                    statusCode = 400;
                }
                else if (secondLineArray[0].Trim() != "HOST")
                {
                    statusCode = 400;
                }

                string hostValue = secondLineArray[1].Trim();
                if (Uri.CheckHostName(hostValue) == UriHostNameType.Unknown)
                {
                    statusCode = 400;
                }
            }
            catch
            {
                statusCode = 400;
            }
            return statusCode;
        }
    }
}
