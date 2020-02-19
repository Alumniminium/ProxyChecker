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
        public static ConcurrentDictionary<uint, List<ushort>> KnownProxies = new ConcurrentDictionary<uint, List<ushort>>();
        public static ConcurrentDictionary<string, List<Proxy>> OrderedProxies = new ConcurrentDictionary<string, List<Proxy>>();
        public static IpLocator Locator = new BinaryDbClient("Assets/ipdb.bin");

        public static void Load(string inputPath, TaskCompletionSource<int> tcs)
        {
            int lineCounter = 0;
            int duplicateCounter = 0;

            using (var Reader = new StreamReader(inputPath))
            {
                while (!Reader.EndOfStream)
                {
                    var line = Reader.ReadLine().Trim();
                    var parts = line.Split(':');

                    if (parts.Length != 2)
                        continue;

                    if (!IPAddress.TryParse(parts[0], out var ipa) || !ushort.TryParse(parts[1], out var port))
                        continue;

                    var proxy = new Proxy(ipa, port);
                    if (Database.IsUnique(proxy))
                        ProxyTester.Proxies.Add(proxy);
                    else
                        duplicateCounter++;

                    lineCounter++;
                }
            }
            Console.WriteLine($"Removing Duplicates: {duplicateCounter}");
            using var writer = new StreamWriter(File.Create(inputPath));
            foreach (var kvp in KnownProxies.OrderBy(c => c.Key))
            {
                foreach (var port in kvp.Value)
                {
                    writer.WriteLine(IpHelper.IntToIp(kvp.Key) + ":" + port);
                }
            }
            tcs.SetResult(lineCounter);
        }

        private static bool IsUnique(Proxy proxy)
        {
            var bytes = proxy.IP.GetAddressBytes();
            var uid = BitConverter.ToUInt32(bytes);

            if (KnownProxies.TryGetValue(uid, out var list))
            {
                if (list.Contains(proxy.Port))
                    return false;
            }
            return KnownProxies.TryAdd(uid, new List<ushort> { proxy.Port });
        }
        public static void Trace(Proxy proxy)
        {
            var location = Locator.Locate(proxy.IP);
            var country = location.Country;

            if (string.IsNullOrEmpty(country))
                country = "Unknown";

            if (!OrderedProxies.ContainsKey(country))
                OrderedProxies.TryAdd(country, new List<Proxy>());

            OrderedProxies[country].Add(proxy);
        }
        internal static void WriteOrderedList()
        {
            using var writer = new StreamWriter("test.txt");
            foreach (var country in OrderedProxies.OrderByDescending(kvp => kvp.Key))
            {
                writer.WriteLine($"[{country.Key}]");
                string working = "Online=";
                string offline = "Offline=";

                foreach (var ip in country.Value.Where(c => c.Alive))
                    working += ip + ",";
                foreach (var ip in country.Value.Where(c => !c.Alive))
                    offline += ip + ",";

                writer.WriteLine(working.AsSpan().Slice(0, working.Length - 1));
                writer.WriteLine(offline.AsSpan().Slice(0, offline.Length - 1));
            }
        }
    }
}
