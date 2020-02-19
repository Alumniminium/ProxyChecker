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
        public override string ToString() => $"{IP}:{Port}";
    }
}
