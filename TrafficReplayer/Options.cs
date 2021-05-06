using CommandLine;

namespace TrafficReplayer
{
    class Options
    {
        [Option('f', "filename", Required = false, HelpText = "Alation csv file name w/header.")]
        public string FileName { get; set; }

        [Option('s', "scope", Required = false, HelpText = "Fury pool url.")]
        public string Scope { get; set; }

        [Option('x', "xauthtoken", Required = false, HelpText = "fury get-token.")]
        public string XAuthToken { get; set; }

        [Option('c', "chunk", Required = false, HelpText = "Paged chunks.")]
        public int Chunk { get; set; }

        [Option('t', "threads", Required = false, HelpText = "Max degree of parallelism.")]
        public int MaxDegreeOfParallelism { get; set; }
    }
}