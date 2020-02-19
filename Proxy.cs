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
        public string Country;
        public int Timeout;

        private static string _responseWithoutProxy = new WebClient().DownloadString("https://her.st").Trim();

        public Proxy(IPAddress ip, ushort port, int timeout)
        {
            IP = ip;
            Port = port;
            Timeout = timeout;
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
            webProxy.BypassProxyOnLocal = false; // only needed if you run on the same host as the website you test against. I do.
            request.Proxy = webProxy;
            request.Timeout = Timeout;
            /// some proxies will not work if you set a User Agent. 
            /// Curious, cause even if I set it to my browser's UA string, they strill refuse it.
            //request.UserAgent = "ProxyTester Version: 1"; 

            return request;
        }

        public override string ToString() => $"{IP}:{Port}";
    }
}
