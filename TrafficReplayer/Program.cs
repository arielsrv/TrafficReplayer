using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using RestSharp;
using static System.Console;

#pragma warning disable 1998


namespace TrafficReplayer
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Action);
        }

        private static async void Action(Options options)
        {
            ShowOptions(options);

            IEnumerable<string> resources = (await File.ReadAllLinesAsync(options.FileName)).ToList();

            RestClient targetClient = new RestClient(options.Scope);

            Parallel.ForEach(resources, new ParallelOptions { MaxDegreeOfParallelism = options.MaxDegreeOfParallelism },
                resource =>
                {
                    var requestTarget = new RestRequest($"{options.Scope}{resource}");
                    WriteLine(resource);
                    requestTarget.AddHeader("x-auth-token", options.XAuthToken);
                    var responseTarget = targetClient.Execute<Response>(requestTarget);
                });

            var i = 0;

            while (true)
            {
                var chunk = resources.Skip(i).Take(options.Chunk);
                List<Task> batch = [];
                WriteLine("Chunk >");

                batch.AddRange(chunk.Select(resource => Task.Run(() =>
                {
                    var requestTarget = new RestRequest($"{options.Scope}{resource}");
                    requestTarget.AddHeader("x-auth-token", options.XAuthToken);
                    var responseTarget = targetClient.Execute<Response>(requestTarget);

                    WriteLine(responseTarget.IsSuccessful ? "ok" : "error");
                })));

                await Task.WhenAll(batch.ToArray());
                WriteLine("Chunk finished. ");
                i += options.Chunk;
                if (i > resources.Count())
                {
                    i = 0;
                }
            }
        }

        private static void ShowOptions(Options options)
        {
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine();
            WriteLine($"file: {options.FileName}");
            WriteLine($"scope: {options.Scope}");
            WriteLine($"threads: {options.MaxDegreeOfParallelism}");
            WriteLine($"chunks: {options.Chunk}");
            WriteLine($"x-auth-token: {options.XAuthToken}");
            WriteLine();
            ResetColor();
        }
    }

    internal class Response
    {
    }
}