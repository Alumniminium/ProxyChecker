using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;

namespace SockPuppet
{
    public static class ProxyTester
    {
        public static Thread[] Threads;
        public static int ThreadCount;
        public static BlockingCollection<Proxy> Proxies;
        public static int Timeout;
        private static TaskCompletionSource<int> Tcs;
        public static Task TestListAsync(string inputPath, string outputPath, int threadCount, int timeout)
        {
            ThreadCount = threadCount;
            Timeout = timeout;
            Tcs = new TaskCompletionSource<int>();
            Proxies = new BlockingCollection<Proxy>(ThreadCount * 2);
            StartThreads(threadCount);
            Database.Load(inputPath, Tcs);
            return Tcs.Task;
        }

        private static void StartThreads(int threadCount)
        {
            Threads = new Thread[threadCount + 1];
            for (int i = 0; i < threadCount; i++)
            {
                Threads[i] = new Thread(WorkLoop);
                Threads[i].IsBackground = true;
                Threads[i].Start();
            }
        }
        private static void WorkLoop()
        {
            foreach (var proxy in Proxies.GetConsumingEnumerable())
            {
                PerformTest(proxy);
                Console.WriteLine($"{(proxy.Alive ? "[ Up! ]" : "[Down!]")}{proxy}");
                Database.Trace(proxy);
            }
        }

        private static void PerformTest(Proxy proxy)
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

        private static HttpWebRequest GenerateRequest(Proxy proxy)
        {
            var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://her.st");
            webProxy.BypassProxyOnLocal = false; // this is gonna run on the her.st server
            request.Proxy = webProxy;
            request.UserAgent = "ProxyTester Version: 1";
            request.Timeout = Timeout;
            return request;
        }
    }
}
