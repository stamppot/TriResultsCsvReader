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
using TriResultsCsvReader.PipelineSteps;

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

                if(null == options.InputFile)
                {
                    Console.WriteLine("No input folder given, using 'uitslagen'");
                    options.InputFile = "uitslagen";
                }
                var inputFiles = GetAllFiles(options.InputFile);

                if (!inputFiles.Any()) { Console.WriteLine("No input file or folder given, will do nothing."); }

                if (options.Verbose) { Console.WriteLine("Files to process:\n"); inputFiles.ForEach(file => Console.WriteLine(file)); }


                var columnsConfig = new ColumnsConfigReader().ReadFile(options.ConfigFile ?? "column_config.xml");

                Expression<Func<ResultRow, bool>> filterExp = null;
                if (string.IsNullOrEmpty(options.MemberFile) && string.IsNullOrEmpty(options.FilterKeywords))
                {
                    var errorMessage = $"No Member file or filter keywords given"; Console.WriteLine(errorMessage);
                }


                var hasMemberFilter = !string.IsNullOrEmpty(options.MemberFile) && ExistsFile(options.MemberFile);
                var hasFilterKeywords = !string.IsNullOrEmpty(options.MemberFile);

                if (!string.IsNullOrEmpty(options.MemberFile) && !ExistsFile(options.MemberFile)) { var errorMessage = $"Member file for filtering not found: {options.MemberFile}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage); }
                else
                {
                    var members = new MemberReaderCsv().Read(options.MemberFile ?? "../../leden2017.csv");
                    var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                    filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));
                }

                //if(hasFilterKeywords)
                //{
                //    var keywords = options.FilterKeywords.Split(new List<char> { '\n', ',', ';' }.ToArray());
                //    var keywordsFilter = new WhitelistFilter(keywords);
                //    filterExp = ((row) =>  keywordsFilter.ContainsMatch(row.Club));
                //}

                if (hasMemberFilter && hasFilterKeywords)
                {
                    var members = new MemberReaderCsv().Read(options.MemberFile);
                    var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                    IEnumerable<string> keywords = new List<string>();
                    if (!string.IsNullOrEmpty(options.FilterKeywords))
                    {
                        keywords = options.FilterKeywords.Split(new List<char> { '\n', ',', ';' }.ToArray()).Select(x => x.Trim());
                    }
                    var keywordsFilter = new WhitelistFilter(keywords);
                    filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam) || keywordsFilter.ContainsMatch(row.Club));
                }



                if (!string.IsNullOrEmpty(options.OutputFolder)) { }


                var resultsReaderCsv = new ResultsReaderCsv(configFile, (str => Console.WriteLine(str)));

                if(string.IsNullOrEmpty(options.OutputFolder))
                {
                    Console.WriteLine("No output folder given, using 'output'");
                    options.OutputFolder = "output/";
                }
                if (!Directory.Exists(options.OutputFolder))
                {
                    Directory.CreateDirectory(options.OutputFolder);
                }

                
                var filteredRaces = new List<StepData>();
                foreach(var file in inputFiles) {
                    var filePath = Path.Combine(options.InputFile, file);
                    Console.WriteLine("filePAth! " + filePath);
                    var raceData = GetRaceFileData(filePath, options.OutputFolder, options.RaceDate);
                    var raceName = raceData.Name;
                    var raceDate = raceData.Date;
      
                    Console.Write("P! race (from filename): {0}, date: {1}, name: {2}", raceName, raceData.Date, raceData.Name);

                    if (options.Verbose)
                    {
                        Console.WriteLine($"Parsing file {file}. Race: {raceName}, date: {raceDate}. Output file: {raceData.OutputFile}");
                    }
                    

                    var stepData = new StepData {InputFile = filePath,
                        Filter = filterExp,
                        ColumnConfigFile = options.ConfigFile, OutputOptions = new List<string> { "csv" } };

                    var readAndFilterStep = new StandardizeHeadersAndFilterStep(columnsConfig, true);
                    
                    var nextStep = readAndFilterStep.Process(stepData);

                    if(nextStep.RaceData.Results.Any())
                        filteredRaces.Add(nextStep);
                }
 
                // order races by newest first
                filteredRaces = filteredRaces.OrderByDescending(r => r.RaceData.Date).ToList();

                var writeStep = new CsvWriterStep();

                foreach (var race in filteredRaces)
                {
                    Console.WriteLine("outputfile: " + race.RaceData.ToFilename());
                    race.OutputFolder = Path.Combine(options.InputFile, options.OutputFolder);
                    writeStep.Process(race);
                }
                
                // output steps

                var htmlOutputStep = new CombineOutputHtmlStep();
                var columns = new ColumnsConfigReader().ReadFile(options.ConfigFile);
                var races = filteredRaces.Select(f => f.RaceData).ToList();

                var outputfile = string.Format("{0}_uitslagen", DateTime.Now.ToString("yyyyMMddhhmm"));
                var output = htmlOutputStep.Process("output", outputfile, columns, races);

                var sqlCreateTableStep = new SqlCreateTableStep();
                var sqlCreateTableStmt = sqlCreateTableStep.Process(columns);
                Console.WriteLine("Create table syntax: " + sqlCreateTableStmt);

                var sqlInsertOutputStep = new CombineOutputSqlInsertStep();
                var outputSqlInsert = sqlInsertOutputStep.Process("output", outputfile, columns, races);
            }
        }



        public class RaceFileData
        {
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public string OutputFile { get; set; }

            public string FullPath { get; set; }
        }

        private static RaceFileData GetRaceFileData(string filename, string outputDir, DateTime raceDate)
        {
            var fi = new FileInfo(filename);

            var index = fi.Name.IndexOf(".csv");
            var name = ReplaceStringMonth(fi.Name);

            RaceFileData output = new RaceFileData() { Name = name };

            if (raceDate.Equals(DateTime.MinValue))
            {
                Console.WriteLine("raceDate is null, will try to get date from filename");
                var datePart = name.Substring(0, 10);
                if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out var racedate))
                {
                    Console.WriteLine("Got raceDate from filename: " + racedate.ToShortDateString());
                    output.Date = racedate;
                    output.Name = name.Substring(9); // remove date
                    raceDate = racedate;
                }
                else
                {
                    //throw new FormatException("Filename must start with date in format yyyyMMdd_racefile.csv");
                }
            }
            else
            {
                Console.WriteLine("Using racedate given by args: " + (raceDate == null ? default(DateTime) : raceDate).ToShortDateString());
                output.Date = raceDate;
            }

            output.Name = output.Name.Replace(".csv", "");
            var raceDateStr = DateTime.MinValue == raceDate ? "" : raceDate.ToString("yyyyMMdd");

            output.FullPath = Path.Combine(fi.Directory.FullName, outputDir);
            output.OutputFile = Path.Combine(fi.Directory.FullName, outputDir, name);
            Console.WriteLine("OutputFile: " + output.OutputFile);
            return output;
        }

        private static string ReplaceStringMonth(string name)
        {
            return name
                .Replace("-jan-", "-01-")
                .Replace("-feb-", "-02-")
                .Replace("-mar-", "-03-")
                .Replace("-apr-", "-04-")
                .Replace("-may-", "-05-")
                .Replace("-jun-", "-06-")
                .Replace("-jul-", "-07-")
                .Replace("-aug-", "-08-")
                .Replace("-sep-", "-09-")
                .Replace("-oct-", "-10-")
                .Replace("-nov-", "-11-")
                .Replace("-dec-", "-12-");
        }

        private static List<string> GetAllFiles(string inputFileOrFolder, bool printVerboseOutput = true)
        {
            var files = new List<string>();

            var dirInfo = new DirectoryInfo(inputFileOrFolder);

            if (printVerboseOutput)
            {
                Console.WriteLine($"Input {inputFileOrFolder}");
            }

            if (dirInfo.Exists)
            {
                Console.WriteLine($"Is a folder {inputFileOrFolder}");
                // a directory is given
                files.AddRange(dirInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly).Select(f => f.FullName).Where(f => !f.EndsWith("_output.csv")));
            }
            else
            {
                Console.WriteLine($"Is a file {inputFileOrFolder}");
                // a file is given
                var fileInfo = new FileInfo(inputFileOrFolder);
                if (!fileInfo.Exists) { var errorMessage = $"Input file does not exist: {inputFileOrFolder}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage); }

                files.Add(fileInfo.FullName);
            }

            if (printVerboseOutput)
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
