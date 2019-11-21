using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace SockPuppet
{
    public class ProxyTester
    {
        public static Thread[] Threads;
        public static int ThreadCount;
        public static BlockingCollection<Proxy> Proxies = new BlockingCollection<Proxy>();
        public static StreamReader Reader;
        public static StreamWriter Writer;
        public static AutoResetEvent LoadBlock = new AutoResetEvent(true);

        public static void TestList(string inputPath, string outputPath, int threadCount, int timeout)
        {
            ThreadCount = threadCount;
            Reader = new StreamReader(inputPath);
            if (!string.IsNullOrEmpty(outputPath))
            {
                Writer = new StreamWriter(outputPath, false);
                Writer.AutoFlush = true;
            }
            StartThreads(threadCount);

            while (!Reader.EndOfStream)
            {
                var line = Reader.ReadLine().Trim();
                var parts = line.Split(':');
                var ip = parts[0];
                var port = ushort.Parse(parts[1]);

                var proxy = new Proxy(ip, port, timeout);
                Proxies.Add(proxy);

                if (Proxies.Count > threadCount * 2)
                    LoadBlock.WaitOne();// don't consume too much ram, list could potentially be gigabytes
            }
        }
        private static void StartThreads(int threadCount)
        {
            Threads = new Thread[threadCount];
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
                if (Proxies.Count < ThreadCount * 2)
                    LoadBlock.Set();

                proxy.Test();

                if (proxy.Alive && proxy.Safe)
                    Writer?.WriteLine(proxy);

                Console.WriteLine($"{(proxy.Alive ? "[ Up! ]" : "[Down!]")}{proxy}");
            }
        }
    }
}
