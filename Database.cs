using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections.Concurrent;
using SockPuppet.IP2Location;
using System.Net;

namespace SockPuppet
{
    public static class Database
    {
        private static ConcurrentDictionary<uint, List<ushort>> KnownProxies = new ConcurrentDictionary<uint, List<ushort>>();
        private static ConcurrentDictionary<string, List<Proxy>> OrderedProxies = new ConcurrentDictionary<string, List<Proxy>>();
        private static IpLocator Locator = new BinaryDbClient("Assets/ipdb.bin");

        public static void LoadProxyList(string inputPath)
        {
            using var Reader = new StreamReader(inputPath);

            while (!Reader.EndOfStream)
            {
                var line = Reader.ReadLine().Trim();
                var parts = line.Split(':');

                if (parts.Length != 2)
                    continue;
                if (!IPAddress.TryParse(parts[0], out var ipa))
                    continue;
                if (!ushort.TryParse(parts[1], out var port))
                    continue;

                var proxy = new Proxy(ipa, port);

                if (Database.IsUnique(proxy))
                    ProxyTester.Enqueue(proxy);
            }
            RemoveDuplicatesAndSave(inputPath);
        }

        public static void AddToSortedList(Proxy proxy)
        {
            if (!OrderedProxies.ContainsKey(proxy.Country))
                OrderedProxies.TryAdd(proxy.Country, new List<Proxy>());

            OrderedProxies[proxy.Country].Add(proxy);
        }

        private static void RemoveDuplicatesAndSave(string inputPath)
        {
            using var writer = new StreamWriter(File.Create(inputPath));
            foreach (var (ip, proxyPorts) in KnownProxies.OrderBy(c => c.Key))
            {
                foreach (var port in proxyPorts)
                    writer.WriteLine(IpHelper.IntToIp(ip) + ":" + port);
            }
        }

        private static bool IsUnique(Proxy proxy)
        {
            var bytes = proxy.IP.GetAddressBytes();
            var ip = BitConverter.ToUInt32(bytes);
            //faster AND more RAM efficient to store the IP as integer

            if (KnownProxies.TryGetValue(ip, out var list))
                return !list.Contains(proxy.Port);

            return KnownProxies.TryAdd(ip, new List<ushort> { proxy.Port });
        }
        public static void Locate(Proxy proxy)
        {
            var location = Locator.Locate(proxy.IP);
            var country = location.Country;

            if (string.IsNullOrEmpty(country))
                country = "Unknown";

            proxy.Country = country;
        }
        public static void ExportIni(string output)
        {
            using var writer = new StreamWriter(output);
            foreach (var (country, proxies) in OrderedProxies.OrderByDescending(kvp => kvp.Key))
            {
                writer.WriteLine($"[{country}]");
                string working = "Online=";
                string offline = "Offline=";

                foreach (var ip in proxies.Where(c => c.Alive))
                    working += ip + ",";
                foreach (var ip in proxies.Where(c => !c.Alive))
                    offline += ip + ",";

                writer.WriteLine(working.AsSpan().Slice(0, working.Length - 1));
                writer.WriteLine(offline.AsSpan().Slice(0, offline.Length - 1));
            }
        }
    }
}
