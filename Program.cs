using System;
using System.Threading;

namespace proxy_checker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "-i", "list.txt", "-o", "working.txt", "-t", "24" };

            var inputs = ParseInput(args);
            ProxyTester.TestList(inputs.input, inputs.output, inputs.threads);

            while (ProxyTester.Proxies.Count > 0)
                Thread.Sleep(1);
        }

        private static (string input, string output, int threads) ParseInput(string[] args)
        {
            var inputIndex = Array.IndexOf(args, "-i");
            var inputFile = args[inputIndex + 1].Trim();

            var outputIndex = Array.IndexOf(args, "-o");
            var outputFile = args[outputIndex + 1].Trim();

            var threadIndex = Array.IndexOf(args, "-t");
            var threadCount = int.Parse(args[threadIndex + 1].Trim());
            return (inputFile, outputFile, threadCount);
        }
    }
}
