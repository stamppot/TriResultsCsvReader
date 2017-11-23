using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TriResultsConsole
{
    class Options
    {

        [Option('f', "read folder")]
        public string InputFolder { get; set; }

        [Option('s', "source file")]
        public string SourceFile { get; set; }

        [Option('o', "output file")]
        public string OutputFile { get; set; }

        [Option('d', "output directory")]
        public string OutputDirectory { get; set; }
    }
}
