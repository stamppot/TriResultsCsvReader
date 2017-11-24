using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace TriResultsConsole
{
    class Options
    {

        [Option('f', "read input folder")]
        public string InputFolder { get; set; }

        [Option('s', "input file")]
        public string InputFile { get; set; }

        [Option('m', "member list csv")]
        public string MemberFile { get; set; }

        [Option('o', "output file")]
        public string OutputFile { get; set; }

        [Option('d', "output directory")]
        public string OutputDirectory { get; set; }

        [Option('u', "url to results page")]
        public string Url { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
