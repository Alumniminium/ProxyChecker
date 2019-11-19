using System;
using System.IO;
using System.Net;

namespace proxy_checker
{
    public class Proxy
    {
        public string IP;
        public ushort Port;
        public bool Alive;

        internal bool Safe() => ogresponse == response;
        private string response;

        // CAREFUL
        private static string ogresponse = new WebClient().DownloadString("https://her.st").Trim();

        public Proxy(string ip, ushort port)
        {
            IP = ip;
            Port = port;
        }

        public void Test()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://her.st");
                var proxy = new WebProxy(IP, Port);
                proxy.BypassProxyOnLocal = false; // this is gonna run on the her.st server
                request.Proxy = proxy;
                request.UserAgent = "ProxyTester Version: 1";
                request.Timeout = 5000;

                WebResponse webResponse = request.GetResponse();
                var reader = new StreamReader(webResponse.GetResponseStream());
                response = reader.ReadToEnd().Trim();
                Alive = true;
            }
            catch
            {
                Alive = false;
            }
        }
        public override string ToString() => $"{IP}:{Port}";
    }
}
