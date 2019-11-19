using System.Linq;
using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace proxy_checker
{
    class Program
    {
        public static Thread[] Threads;
        public static BlockingCollection<Proxy> Proxies = new BlockingCollection<Proxy>();
        public static StreamWriter Writer;
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "-i", "list.txt", "-o", "working.txt", "-t", "24" };

            ParseInput(args);
            while (Proxies.Count > 0)
                Thread.Sleep(1);
        }

        private static void ParseInput(string[] args)
        {
            var inputIndex = Array.IndexOf(args, "-i");
            var input = args[inputIndex + 1].Trim();

            var outputIndex = Array.IndexOf(args, "-o");
            var output = args[outputIndex + 1].Trim();

            var threadIndex = Array.IndexOf(args, "-t");
            var threadCount = int.Parse(args[threadIndex + 1].Trim());

            StartThreads(threadCount);

            Writer = new StreamWriter(output, false);
            Writer.AutoFlush = true;

            var file = File.ReadAllLines(input);
            foreach (var line in file)
            {
                var vp = line.Split(':');
                if (vp.Length == 2)
                {
                    var ip = vp[0];
                    var port = ushort.Parse(vp[1]);
                    var proxy = new Proxy(ip, port);
                    Proxies.Add(proxy);
                }
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
                proxy.Test();

                if (proxy.Alive)
                    Writer.WriteLine(proxy);

                Console.WriteLine($"{( proxy.Alive ? "[ Up! ]" : "[Down!]")}{proxy}");
            }
        }
    }
}
