using System.IO;
using System.Net;

namespace SockPuppet
{
    public class Proxy
    {
        public string IP;
        public ushort Port;
        public int Timeout;
        public bool Alive;
        internal bool Safe => _responseWithoutProxy == _response;
        private string _response;
        private static string _responseWithoutProxy = new WebClient().DownloadString("https://her.st").Trim();

        public Proxy(string ip, ushort port, int timeout)
        {
            IP = ip;
            Port = port;
            Timeout = timeout;
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
                request.Timeout = Timeout;

                WebResponse webResponse = request.GetResponse();
                var reader = new StreamReader(webResponse.GetResponseStream());
                _response = reader.ReadToEnd().Trim();
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
