using System.IO;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace SockPuppet
{
    public static class ProxyTester
    {
        private static Thread[] Threads;
        private static BlockingCollection<Proxy> Proxies;
        private static StreamReader Reader;
        private static StreamWriter Writer;
        private static string InputPath, OutputPath;
        private static int Timeout, threads_done;
        private static bool doneReading;

        public static async Task TestAsync(string input, string output, int threadCount, int timeout)
        {
            InputPath = input;
            OutputPath = output;
            Timeout = timeout;
            Writer = new StreamWriter(output, false);
            Writer.AutoFlush = true;
            Proxies = new BlockingCollection<Proxy>(threadCount * 2);
            Threads = new Thread[threadCount + 1];

            for (int i = 0; i < threadCount - 1; i++)
            {
                Threads[i] = new Thread(WorkLoop);
                Threads[i].IsBackground = true;
                Threads[i].Start();
            }

            ReadLoop();
            while (threads_done < threadCount)
                await Task.Delay(1);
        }

        private static void WorkLoop()
        {
            foreach (var proxy in Proxies.GetConsumingEnumerable())
            {
                proxy.PerformTest(proxy);
                Console.WriteLine($"{(proxy.Alive ? "[ Up! ]" : "[Down!]")}{proxy}");
                Database.Locate(proxy);
                Database.AddToSortedList(proxy);

                if (proxy.Alive)
                {
                    lock (Writer)
                        Writer.WriteLine(proxy.IP + ":" + proxy.Port);
                }

                if (doneReading && Proxies.Count == 0)
                    break;
            }
            Interlocked.Increment(ref threads_done);
        }
        private static void ReadLoop()
        {
            Reader = new StreamReader(InputPath);
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

                var proxy = new Proxy(ipa, port, Timeout);
                if (Database.IsUnique(proxy))
                    Proxies.Add(proxy);
            }
            doneReading = true;
        }
    }
}
