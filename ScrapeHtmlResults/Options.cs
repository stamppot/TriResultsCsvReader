using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeHtmlResults
{
    class Options
    {
        [Option('f', "input file or folder")]
        public string InputFile { get; set; }

    }
}
