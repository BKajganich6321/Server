using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnWebServer
{
    public static class Validator
    {
        public static bool CheckArgs(string[] args)
        {
            bool valid = false;
            Int32 webPort;
            IPAddress webIP;
            string webRoot;

            string root;
            string ip;
            string port;

            if (args.Length == 3)
            {
                string[] rootArg = args[0].Split('=');
                string[] ipArg = args[1].Split('=');
                string[] portArg = args[2].Split('=');

                Console.WriteLine("\n{0} \n{1}\n{2}", rootArg[0], ipArg[0], portArg[0]);

                string buff = rootArg[0];
                if (buff != "-webRoot")
                {
                    return false;
                }
                if (ipArg[0] != "-webIP")
                {
                    return false;
                }
                if (portArg[0] != "-webPort")
                {
                    return false;
                }
                else
                {
                    webRoot = rootArg[1];
                    port = portArg[1];
                    ip = ipArg[1];
                    try
                    {
                        if (!Directory.Exists(webRoot))
                        {
                            return false;
                        }
                        webPort = Int32.Parse(portArg[1]);

                    }
                    catch { return false; }
                    valid = IPAddress.TryParse(ipArg[1], out webIP);
                }
            }
            else
            {
                return false;
            }
            return valid;
        }
    }
}

