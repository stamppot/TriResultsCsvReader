using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using FileAppServices;
using Optional.Unsafe;
using TriResultsCsvReader;
using TriResultsCsvReader.PipelineSteps;
using TriResultsCsvReader.Utils;

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

                var fileUtils = new FileUtils();
                var inputFiles = fileUtils.GetAllFiles(options.InputFile);

                if (!inputFiles.Any()) { Console.WriteLine("No input file or folder given, will do nothing."); }

                if (options.Verbose) { Console.WriteLine("Files to process:\n"); inputFiles.ForEach(file => Console.WriteLine(file)); }


                var columnsConfig = new ColumnsConfigReader().ReadFile(options.ConfigFile ?? "column_config.xml").ToList();

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



                if(string.IsNullOrEmpty(options.OutputFolder))
                {
                    Console.WriteLine("No output folder given, using 'output'");
                    options.OutputFolder = "output/";
                }
                if (!Directory.Exists(options.OutputFolder))
                {
                    Directory.CreateDirectory(options.OutputFolder);
                }


                /// Read and filter races

                var infoLogs = new List<string>();

                var filteredRaces = new List<RaceEnvelope>();
                foreach(var file in inputFiles) {
                    var filePath = Path.Combine(options.InputFile, file);
                    Console.WriteLine("filePath: " + filePath);
                    var raceData = new RaceDataFileUtils().GetRaceDataFromFilename(filePath);
      
                    if (options.Verbose)
                    {
                        Console.WriteLine($"Parsing file {file}. Race: {raceData.ValueOrDefault().Name}, date: {raceData.ValueOrDefault().Date}. ");
                    }

                    var stepData = new RaceEnvelope {InputFile = filePath, OutputOptions = new List<string> { "csv" } };

                    var readAndStandardizeStepStep = new GetRaceDataStep(columnsConfig, infoLogs);
                    
                    var nextStep = readAndStandardizeStepStep.Process(stepData);

                    var filterStep = new FilterReduceStep(columnsConfig, filterExp, infoLogs);

                    nextStep = filterStep.Process(nextStep);

                    if(nextStep.RaceData.Results.Any())
                        filteredRaces.Add(nextStep);
                }
 
                // order races by newest first
                filteredRaces = filteredRaces.OrderByDescending(r => r.RaceData.Date.ValueOr(DateTime.MinValue)).ToList();


                /// Write normalized race data (columns are normalized, results are filtered) output as csv. One file per race.
                var writeStep = new CsvWriterStep();

                foreach (var race in filteredRaces)
                {
                    Console.WriteLine("outputfile: " + race.RaceData.ToFilename());
                    race.OutputFolder = Path.Combine(options.InputFile, options.OutputFolder);
                    writeStep.Process(race);
                }
                
                /// Combined output steps

                var htmlOutputStep = new CombineOutputHtmlStep();
                var columns = new ColumnsConfigReader().ReadFile(options.ConfigFile);
                var races = filteredRaces.Select(f => f.RaceData).ToList();

                var outputfile = string.Format("{0}_uitslagen", DateTime.Now.ToString("yyyyMMddhhmm"));
                var output = htmlOutputStep.Process("output", outputfile, columns, races);

                options.OutputSql = true; // for now, always set it to true, since it's easier when running from within VS
                if (options.OutputSql)
                {
                    var sqlCreateTableStep = new SqlCreateTableStep();
                    var sqlCreateTableStmt = sqlCreateTableStep.Process(columns);
                    Console.WriteLine("Create table syntax: " + sqlCreateTableStmt);

                    var sqlInsertOutputStep = new CombineOutputSqlInsertStep();
                    var outputSqlInsert = sqlInsertOutputStep.Process("output", outputfile, columns, races);
                }
            }
        }


     

        private static bool ExistsFile(string filename)
        {
            return File.Exists(filename);
        }
    }
}
