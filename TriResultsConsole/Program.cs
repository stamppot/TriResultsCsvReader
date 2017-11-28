using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriResultsCsvReader;

namespace TriResultsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            var IsValid = CommandLine.Parser.Default.ParseArguments(args, options);

            if (IsValid)
            {
                // Values are available here
                if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFile);

                var configFile = string.IsNullOrEmpty(options.ConfigFile) ? "column_config.xml" : options.ConfigFile;
                if (!ExistsFile(configFile)) Console.WriteLine("Config file not found: {0}", configFile);

                if (string.IsNullOrEmpty(options.MemberFile)) { Console.WriteLine("Member file not given, cannot filter results."); }

                var inputFiles = GetAllFiles(options.InputFile);

                if (!inputFiles.Any()) { Console.WriteLine("No input file or folder given, will do nothing."); }

                if (options.Verbose) { Console.WriteLine("Files to process:\n"); inputFiles.ForEach(file => Console.WriteLine(file)); }


                Expression<Func<ResultRow, bool>> filterExp = null;
                if (!string.IsNullOrEmpty(options.MemberFile) && !ExistsFile(options.MemberFile)) { var errorMessage = $"Member file for filtering not found: {options.MemberFile}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage); }
                else
                {
                    var members = new MemberReaderCsv().Read(options.MemberFile);
                    var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                    //if(options.Verbose) { Console.WriteLine("Filter list:\n" + String.Join("\n", memberWhitelist.GetEntries())); }
                    filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));
                }


                if (!string.IsNullOrEmpty(options.OutputFile)) { }


                var resultsReaderCsv = new ResultsReaderCsv(configFile, (str => Console.WriteLine(str)));
                var resultsWriterCsv = new TriResultsCsvWriter(configFile, (str => Console.WriteLine(str)));

                foreach (var file in inputFiles)
                {
                    var raceData = GetRaceData(file, options.RaceDate);
                    var raceName = options.RaceName ?? raceData.Name;
                    var raceDate = raceData.Date;

                    if (options.Verbose)
                    {
                        Console.WriteLine($"Parsing file {file}. Race: {raceName}, date: {raceDate}. Output file: {raceData.OutputFile}");
                    }
                    resultsWriterCsv.StandardizeCsv(raceName, raceDate, file, raceData.OutputFile, filterExp);
                }
            }
        }
        

        public class RaceData
        {
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public string OutputFile { get; set; }
        }

        private static RaceData GetRaceData(string filename, DateTime raceDate)
        {
            var fi = new FileInfo(filename);

            var index = fi.Name.IndexOf(".csv");
            var name = fi.Name;

            RaceData output = new RaceData() { Name = name };

            if(raceDate.Equals(DateTime.MinValue))
            {
                Console.WriteLine("raceDate is null, will try to get date from filename");
                if(DateTime.TryParseExact(name.Substring(0, 8), "yyyyMMdd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out var racedate))
                {
                    Console.WriteLine("Got raceDate from filename: " + racedate.ToShortDateString());
                    output.Date = racedate;
                    output.Name = name.Substring(9); // remove date
                    raceDate = racedate;
                }
                else
                {
                    throw new FormatException("Filename must start with date in format yyyyMMdd_racefile.csv");
                }
            }
            else
            {
                Console.WriteLine("Using racedate given by args: " + (raceDate == null ? default(DateTime) : raceDate).ToShortDateString());
                output.Date = raceDate;
            }

            output.OutputFile = Path.Combine(fi.Directory.FullName, raceDate.ToString("yyyyMMdd") + "_" + name.Insert(index, "_output"));

            return output;
        }

        private static List<string> GetAllFiles(string inputFileOrFolder, bool printVerboseOutput = true)
        {
            var files = new List<string>();

            var dirInfo = new DirectoryInfo(inputFileOrFolder);

            if (printVerboseOutput) {
                Console.WriteLine($"Input {inputFileOrFolder}");
            }

            if(dirInfo.Exists)
            {
                Console.WriteLine($"Is a folder {inputFileOrFolder}");
                // a directory is given
                files.AddRange(dirInfo.GetFiles("*.csv", SearchOption.AllDirectories).Select(f => f.FullName).Where(f => !f.EndsWith("_output.csv")));
            }
            else
            {
                Console.WriteLine($"Is a file {inputFileOrFolder}");
                // a file is given
                var fileInfo = new FileInfo(inputFileOrFolder);
                if (!fileInfo.Exists) { var errorMessage = $"Input file does not exist: {inputFileOrFolder}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage); }

                files.Add(fileInfo.FullName);
            }

            if(printVerboseOutput)
            {
                Console.WriteLine("Found files:\n");
                files.ForEach(f => Console.WriteLine($"{f}"));
            }

            return files;
        }

        private static bool ExistsFile(string filename)
        {
            return File.Exists(filename);
        }
    }
}
