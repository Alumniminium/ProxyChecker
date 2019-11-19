using System;
using System.Threading;

namespace proxy_checker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "-input", "list.txt", "-output", "working.txt", "-threads", "24", "-timeout", "1000" };

            var inputs = ParseInput(args);
            ProxyTester.TestList(inputs.input, inputs.output, inputs.threads, inputs.timeout);

            while (ProxyTester.Proxies.Count > 0)
                Thread.Sleep(1);
        }

        private static (string input, string output, int threads, int timeout) ParseInput(string[] args)
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
