using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriResultsCsvReader;

namespace TriResultsForm
{
    public class ProgramOptions : IOptions
    {
        //[Option('i', "input file or folder")]
        public string InputFolderOrFile { get; set; }

        //[Option('m', "member list csv")]
        public string MemberFile { get; set; }

        //[Option('x', HelpText = "Filter keywords")]
        public string FilterKeywords { get; set; }

        //[Option('c', HelpText = "Config file", DefaultValue = "column_config.xml")]
        public string ConfigFile { get; set; }

        //[Option('o', "output folder")]
        public string OutputFolder { get; set; }


        //[Option('s', "output to sql file")]
        public bool OutputSql { get; set; }

        public bool OutputCsv { get; set; }

        public bool OutputHtml { get; set; }

        public bool Verbose { get; set; }

        //[Option('d', "race date")]
        public DateTime RaceDate { get; set; }

        public string RaceName { get; set; }
    }
}
