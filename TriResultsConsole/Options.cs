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
        [Option('i', "input file or folder")]
        public string InputFile { get; set; }

        [Option('m', "member list csv")]
        public string MemberFile { get; set; }

        [Option('x', HelpText = "Filter keywords")]
        public string FilterKeywords { get; set; }

        [Option('c', HelpText = "Config file", DefaultValue = "column_config.xml")]
        public string ConfigFile { get; set; }

        [Option('o', "output folder")]
        public string OutputFolder { get; set; }


        [Option('s', "output to sql file")]
        public bool OutputSql { get; set; }

        //[Option('d', "output directory")]
        //public string OutputDirectory { get; set; }


        //[Option('r', "race name")]
        //public string RaceName { get; set; }


        [Option('d', "race date")]
        public DateTime RaceDate { get; set; }


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
