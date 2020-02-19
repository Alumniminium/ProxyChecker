using System.IO;
using System.Net;

namespace SockPuppet
{
    public class Proxy
    {
        public IPAddress IP;
        public ushort Port;
        public bool Alive;
        internal bool Safe => _responseWithoutProxy == response;
        public string response;
        private static string _responseWithoutProxy = new WebClient().DownloadString("https://her.st").Trim();

        public Proxy(IPAddress ip, ushort port)
        {
            IP = ip;
            Port = port;
        }
        public void PerformTest(Proxy proxy)
        {
            try
            {
                var request = GenerateRequest(proxy);
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                proxy.response = reader.ReadToEnd();
                proxy.Alive = true;
            }
            catch
            {
                proxy.Alive = false;
            }
        }

        private HttpWebRequest GenerateRequest(Proxy proxy)
        {
            var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://her.st");
            webProxy.BypassProxyOnLocal = false; // this is gonna run on the her.st server
            request.Proxy = webProxy;
            //request.UserAgent = "ProxyTester Version: 1";
            return request;
        }

        public override string ToString() => $"{IP}:{Port}";
    }
}
