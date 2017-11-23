using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            var IsValid = CommandLine.Parser.Default.ParseArguments(args, options);
        }
    }
}
