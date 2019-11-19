using System;
using System.Threading;
using System.Reflection;

namespace SockPuppet
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "-input", "list.txt", "-output", "working.txt", "-threads", "24", "-timeout", "10000" };

            try
            {
                var (input, output, threads, timeout) = ParseInput(args);
                ProxyTester.TestList(input, output, threads, timeout);

                while (ProxyTester.Proxies.Count > 0)
                    Thread.Sleep(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var ass = Assembly.GetExecutingAssembly().GetName();
                Console.WriteLine($"{ass.Name}, Version {ass.Version} Arch: {ass.ProcessorArchitecture}");
                Console.WriteLine();
                Console.WriteLine($"Usage: {ass.Name} -input [1] -output [2] -threads [3] -timeout [4]");
                Console.WriteLine("1: path to proxy list (format: ip:port)");
                Console.WriteLine("2: path to output file containing all the working proxies");
                Console.WriteLine("3: amount of proxies to try at the same time (sane values: [5])");
                Console.WriteLine("4: how long to wait for a response before giving up in milliseconds (sane values: [5])");
                Console.WriteLine("5: Sane thread counts: 8-16. Sane Timeout durations: 1000-10000");
                Console.WriteLine("1000 milliseconds = 1 second");
            }
        }

        private static (string, string, int, int) ParseInput(string[] args)
        {
            var inputIndex = Array.IndexOf(args, "-input");
            var inputFile = args[inputIndex + 1].Trim();

            var outputIndex = Array.IndexOf(args, "-output");
            var outputFile = args[outputIndex + 1].Trim();

            var threadIndex = Array.IndexOf(args, "-threads");
            var threadCount = int.Parse(args[threadIndex + 1].Trim());

            var timeoutIndex = Array.IndexOf(args, "-timeout");
            var timeout = int.Parse(args[timeoutIndex + 1].Trim());

            return (inputFile, outputFile, threadCount, timeout);
        }
    }
}
